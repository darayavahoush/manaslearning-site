using BlazorBootstrap;
using manaslearning.Admin.UI.Models;
using manaslearning.Admin.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net;

namespace manaslearning.Admin.UI.Pages.Users;

public partial class Index
{
    #region Members

    private Offcanvas uploadOffcanvas;
    private Offcanvas resetPasswordOffcanvas;

    private bool isUploadInProgress;
    private bool isResetInProgress;
    private string resetEmail;
    private string resetPassword;

    private ResetPasswordState resetPasswordState = ResetPasswordState.None;

    #endregion

    #region Propeties

    //[Inject] private ILogger Logger { get; set; }

    [Inject] private IUserService _userService { get; set; }

    public IEnumerable<UserInfo> Users { get; set; }

    #endregion Properties

    #region Methods

    protected override async Task OnInitializedAsync()
    {
        await LoadUsersAsync();
    }

    private async Task ShowBulkUploadAsync()
    {
        if (!isUploadInProgress)
            await uploadOffcanvas.ShowAsync();
    }

    private async Task OnCompletedAsync()
    {
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            isUploadInProgress = true;
            // Users
            //var apiResponseModel = await _userService.GetUsersAsync();
            //if (!apiResponseModel.Succeeded && apiResponseModel.StatusCode == HttpStatusCode.NotFound)
            //{
            //    this.Users = null;
            //    return;
            //}
            //else if (!apiResponseModel.Succeeded || !(apiResponseModel.Data is IEnumerable<UserInfo>))
            //{
            //    // handle failed response
            //    return;
            //}

            //var users = apiResponseModel.Data as IEnumerable<UserInfo>;
            //this.Users = users?.OrderBy(x => x.OrganizationName)?.ThenBy(x => x.FirstName)?.ThenBy(x => x.LastName);

            try
            {
                var users = await _userService.GetUsersAsync();
                this.Users = users?.OrderBy(x => x.OrganizationName)?.ThenBy(x => x.FirstName)?.ThenBy(x => x.LastName);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
        catch (Exception ex)
        {
            //Logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=Pages.Users.Index.OnInitializedAsync");
        }
        finally
        {
            isUploadInProgress = false;
        }
    }

    #region Reset Password

    private async Task ShowResetPasswordAsync(string email)
    {
        if (!isResetInProgress)
        {
            resetEmail = email;
            await resetPasswordOffcanvas.ShowAsync();
        }
        else
            resetEmail = string.Empty;
    }

    private async Task HideResetPasswordAsync()
    {
        await resetPasswordOffcanvas.HideAsync();
    }

    private async Task ResetPasswordAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                resetPassword = string.Empty;
                resetPasswordState = ResetPasswordState.Failed;
                return;
            }

            isResetInProgress = true;
            var apiResponseModel = await _userService.ResetPasswordAsync(email);
            if (!apiResponseModel.Succeeded && apiResponseModel.StatusCode == HttpStatusCode.NotFound)
            {
                resetPassword = string.Empty;
                resetPasswordState = ResetPasswordState.Failed;
                return;
            }
            else if (!apiResponseModel.Succeeded || !(apiResponseModel.Data is string))
            {
                resetPassword = string.Empty;
                resetPasswordState = ResetPasswordState.Failed;
                return;
            }

            resetPassword = $"{apiResponseModel.Data}";
            resetPasswordState = ResetPasswordState.Generated;
            isResetInProgress = false;
        }
        catch (Exception ex)
        {
            resetPassword = string.Empty;
            resetPasswordState = ResetPasswordState.Failed;
            isResetInProgress = false;
            //Logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=Pages.Users.Index.OnInitializedAsync");
        }
    }

    private void OnResetPasswordOffcanvasClose()
    {
        resetEmail = string.Empty;
        resetPassword = string.Empty;
        resetPasswordState = ResetPasswordState.None;
        isResetInProgress = false;
    }

    #endregion Reset Password

    #endregion Methods
}
