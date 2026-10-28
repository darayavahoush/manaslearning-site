using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IOrganizationRepository
    {
        Task<int> CreateOrganizationAsync(
                long? userId,
                string name,
                string displayName,
                int? parentOrganizationId,
                int organizationTypeId
            );

        Task<int> UpdateOrganizationAsync(
                long? userId,
                string name,
                string dispalyName,
                int organizationType,
                int? parentOrganizationId,
                int? defaultAdmin,
                Guid oid
            );

        Task<int> SetOranizationDefaultAdminAsync(
                int organizationId,
                long userId,
                long updatedBy            
            );

        Task<Organization> GetOrganizationByNameAsync(string name);

        Task<Organization> GetOrganizationByOidAsync(Guid oid);

        Task<Organization> GetOrganizationAsync(int organizationId);

        Task<IEnumerable<Organization>> GetOrganizationsAsync();

        Task<IEnumerable<Organization>> GetAllOrganizationsAsync();

        Task<int> DeleteOrganizationAsync(Guid oid, int userId);

    }
}