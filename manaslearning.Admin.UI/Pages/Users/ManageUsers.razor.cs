using BlazorBootstrap;
using manaslearning.Admin.UI.Models;
using manaslearning.Admin.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace manaslearning.Admin.UI.Pages.Users;

public partial class ManageUsers
{
    #region Members

    private IEnumerable<OrganizationInfo> organizations;

    #endregion

    #region Properties

    [Inject] private IOrganizationService _organizationService { get; set; }
    [Inject] private IOrganizationUserService _organizationUserService { get; set; }
    [Parameter] public string? OrganizationOid { get; set; }

    #endregion

    #region Methods

    protected async Task OnInitializedAsync()
    {
        organizations = await _organizationService.GetOrganizationsAsync();
    }

    private async Task<GridDataProviderResult<OrganizationUserInfo>> OrganizationUsersDataProvider(GridDataProviderRequest<OrganizationUserInfo> request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(OrganizationOid) && !Guid.TryParse(OrganizationOid, out Guid oid))
                return (new GridDataProviderResult<OrganizationUserInfo> { Data = new List<OrganizationUserInfo>(), TotalCount = 0 });

            var result = await _organizationUserService.GetUsersByOrganizationOidAsync(Guid.Parse(OrganizationOid));
            return await Task.FromResult(new GridDataProviderResult<OrganizationUserInfo> { Data = result, TotalCount = result.Count() });
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        catch (HttpRequestException)
        {
            // TODO: update this
        }
        catch (Exception)
        {
            throw; // TODO: update this
        }

        return (new GridDataProviderResult<OrganizationUserInfo> { Data = new List<OrganizationUserInfo>(), TotalCount = 0 });
    }

    #endregion
}
