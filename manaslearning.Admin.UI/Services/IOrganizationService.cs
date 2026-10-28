using manaslearning.Admin.UI.Models;

namespace manaslearning.Admin.UI.Services;

public interface IOrganizationService
{
    public Task<IEnumerable<OrganizationInfo>> GetOrganizationsAsync();

    public Task<IEnumerable<OrganizationInfo>> GetAllOrganizationsAsync();

    public Task<OrganizationInfo> GetOrganizationByOidAsync(Guid oid);

    public Task<OrganizationInfo> GetOrganizationAsync(string organizationId);

    public Task<ApiResponseModel> CreateOrganizationAsync(string name, string displayName, int type);

    public Task<ApiResponseModel> UpdateOrganizationAsync(Guid oid, string name, string displayName, int type);
}
