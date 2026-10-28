using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IAddressRepository
    {
        Task<int> CreateAddressAsync(
                long userId,
                int organizationId,
                int addressTypeId,
                string name,
                string addressLine1,
                string addressLine2,
                string addressLine3,
                string city,
                string state,
                string country,
                string zipCode,
                bool isDefault
            );

        Task<int> UpdateAddressAsync(
                Guid oid,
                long userId,
                int organizationId,
                int addressTypeId,
                string name,
                string addressLine1,
                string addressLine2,
                string addressLine3,
                string city,
                string state,
                string country,
                string zipCode,
                bool isDefault
            );

        Task<IEnumerable<Address>> GetAddressesAsync(int organizationId);

        Task<Address> GetAddressAsync(int organizationId, int addressId);

        Task<Address> GetAddressByOidAsync(int organizationId, Guid oid);

        Task<int> DeleteAddressAsync(int organizationId, Guid oid);
    }
}
