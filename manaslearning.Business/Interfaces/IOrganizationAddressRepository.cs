using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IOrganizationAddressRepository
    {
        Task<int> CreateOrganizationAddressAsync(
            int organizationId,
            int addressTypeId,
            string addressLine1,
            string addressLine2,
            string addressLine3,
            string city,
            string state,
            string country,
            string zipCode,
            string phone1,
            string phone2,
            bool isDefault
        );

        Task<int> UpdateOrganizationAddressAsync(
            int addressId,
            int organizationId,
            int addressTypeId,
            string addressLine1,
            string addressLine2,
            string addressLine3,
            string city,
            string state,
            string country,
            string zipCode,
            string phone1,
            string phone2,
            bool isDefault
        );

        Task<IEnumerable<OrganizationAddress>> GetOrganizationAddressesAsync(int organizationId);

        Task<IEnumerable<OrganizationAddress>> GetAllOrganizationAddressesByOrganizationOidAsync(Guid organizationOid);

        Task<IEnumerable<OrganizationAddress>> GetOrganizationAddressesByOrganizationOidAsync(Guid organizationOid);

        Task<OrganizationAddress> GetOrganizationAddressAsync(Guid organizationOid, Guid organizationAddressOid);

        Task<int> DeleteOrganizationAddressAsync(Guid oid);
    }
}
