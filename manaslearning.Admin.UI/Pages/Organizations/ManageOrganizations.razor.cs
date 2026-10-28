using BlazorBootstrap;
using manaslearning.Admin.UI.Models;
using manaslearning.Admin.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace manaslearning.Admin.UI.Pages.Organizations;

public partial class ManageOrganizations
{
    #region Members

    private Grid<OrganizationInfo> organizationGrid;
    private Offcanvas offcanvas;
    private Guid? organizationOid;

    #endregion

    #region Properties

    [Inject] private IOrganizationService _organizationService { get; set; }

    #endregion

    #region Methods

    private async Task<GridDataProviderResult<OrganizationInfo>> OrganizationsDataProvider(GridDataProviderRequest<OrganizationInfo> request)
    {
        try
        {
            var result = await _organizationService.GetAllOrganizationsAsync();
            return await Task.FromResult(new GridDataProviderResult<OrganizationInfo> { Data = result, TotalCount = result.Count() });
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        catch (Exception)
        {
            throw; // TODO: update this
        }

        return (new GridDataProviderResult<OrganizationInfo> { Data = new List<OrganizationInfo>(), TotalCount = 0 });
    }

    private async Task OnShowOffcanvasClick()
    {
        Reset();
        await offcanvas?.ShowAsync();
    }

    private async Task OnHideOffcanvasClick()
    {
        Reset();
        await offcanvas?.HideAsync();
    }

    private async Task HandleOrganizationChanged()
    {
        await organizationGrid.RefreshDataAsync();
    }

    private async Task EditOrganization(Guid oid)
    {
        organizationOid = oid;
        await offcanvas?.ShowAsync();
    }

    private void Reset()
    {
        organizationOid = null;
    }

    #endregion
}
