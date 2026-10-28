using manaslearning.Admin.UI.Models;

namespace manaslearning.Admin.UI.Services;

public interface IOrganizationUserService
{
    public Task<IEnumerable<OrganizationUserInfo>> GetAllUsersByOrganizationOidAsync(Guid Oid);
    public Task<IEnumerable<OrganizationUserInfo>> GetUsersByOrganizationOidAsync(Guid Oid);
    public Task<OrganizationUserInfo> GetUserAsync(Guid OrganizationOid, long userId);
}
