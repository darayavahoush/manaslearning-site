using BlazorBootstrap;
using manaslearning.Admin.UI.Models;
using manaslearning.Admin.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.ComponentModel.DataAnnotations;

namespace manaslearning.Admin.UI.Pages.Organizations;

public partial class CreateUpdateOrganization : IDisposable
{
    #region Members

    private Guid? organizationOid;
    private bool isEdit = false;

    private OrganizationForm organizationForm { get; set; } = new();
    private EditContext? editContext;
    private bool formInvalid = false;

    private bool isLoading = false;

    private List<ToastMessage> messages = new();

    private ConfirmDialog dialog;

    #endregion

    #region Properties

    [Inject] private IOrganizationService _organizationService { get; set; }

    [CascadingParameter]
    protected Guid? OrganizationOid
    {
        get { return organizationOid; }
        set { organizationOid = value; }
    }

    [Parameter] public EventCallback OnOrganizationChanged { get; set; }

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        editContext = new(organizationForm);
        editContext.SetFieldCssClassProvider(new CustomFieldClassProvider());
        editContext.OnFieldChanged += HandleFieldChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (OrganizationOid.HasValue)
        {
            isEdit = true;
            var org = await _organizationService.GetOrganizationByOidAsync(organizationOid.Value);
            organizationForm = new() { Oid = org.Oid, OrganizationId = org.OrganizationId, ParentOrganizationId = org.ParentOrganizationId, Name = org.Name, DisplayName = org.DisplayName, OrganizationType = $"{org.OrganizationType}" };
        }
        else
        {
            isEdit = false;
            organizationForm = new();
        }
        editContext = new(organizationForm);
        formInvalid = false;
    }

    private void HandleFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (editContext != null)
        {
            formInvalid = !editContext.Validate();
            StateHasChanged();
        }
    }

    private void HandleValidSubmit() => dialog?.Show();

    private void HandleInvalidSubmit() => formInvalid = true;

    private async Task SaveOrganization()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            if (editContext != null && editContext.Validate())
            {
                editContext.MarkAsUnmodified();

                var form = editContext.Model as OrganizationForm;
                if (form == null)
                {
                    ShowDangerAlert("Something went wrong please refresh the page and try again.");
                    return;
                }

                if (!int.TryParse(form.OrganizationType, out int type) || type < 0 || type > 3)
                {
                    ShowDangerAlert("Something went wrong please refresh the page and try again.");
                    return;
                }

                ApiResponseModel result;
                if (!isEdit)
                    result = await _organizationService.CreateOrganizationAsync(form.Name, form.DisplayName, type);
                else
                    result = await _organizationService.UpdateOrganizationAsync(form.Oid, form.Name, form.DisplayName, type);

                if (result.Succeeded)
                {
                    await OnOrganizationChanged.InvokeAsync();
                    if (isEdit)
                        ShowSuccessAlert("Organization updated successfully.");
                    else
                        ShowSuccessAlert("Organization created successfully.");
                }
                else
                    ShowDangerAlert($"{result.Data}");
            }
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task OnYesAsync()
    {
        await SaveOrganization();
    }

    private async Task OnNoAsync() => dialog?.Hide();

    private void ShowSuccessAlert(string message)
    {
        messages?.Add(new() { Type = ToastType.Success, Message = message, });
    }

    private void ShowDangerAlert(string message)
    {
        messages?.Add(new() { Type = ToastType.Danger, Message = message, });
    }

    public void Dispose()
    {
        if (editContext != null)
        {
            editContext.OnFieldChanged -= HandleFieldChanged;
        }
    }

    #endregion

    public class OrganizationForm
    {
        public Guid Oid { get; set; }

        public int OrganizationId { get; set; }

        public int? ParentOrganizationId { get; set; }

        [Required(ErrorMessage = "Please provide a name")]
        [StringLength(500, ErrorMessage = "Max. 500 characters allowed.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Please provide display name")]
        [StringLength(500, ErrorMessage = "Display Name - Max. 500 characters allowed.")]
        public string DisplayName { get; set; } = default!;

        [Required(ErrorMessage = "Please select organization type.")]
        [StringLength(1, ErrorMessage = "Invalid organization type.")]
        public string OrganizationType { get; set; } = string.Empty; // 1: School, 2: Hospital, 3: Individual

        public bool IsActive { get; set; }
    }
}
