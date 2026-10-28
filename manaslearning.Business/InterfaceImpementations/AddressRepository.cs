using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class AddressRepository : IAddressRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public AddressRepository(IApplicationConnection applicationConnection, ILogger<AddressRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        /// <summary>
        /// Create a address
        /// </summary>        
        /// <param name="userId"></param>  
        /// <param name="organizationId"></param>  
        /// <param name="addressTypeId"></param>
        /// <param name="name"></param>
        /// <param name="addressLine1"></param>
        /// <param name="addressLine2"></param>
        /// <param name="addressLine3"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="country"></param>
        /// <param name="zipCode"></param>
        /// <param name="isDefault"></param>        
        /// <returns>
        /// int
        ///     -1:     Error     
        /// </returns>
        public async Task<int> CreateAddressAsync(            
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
        )
        {
            string procedureName = "[dbo].[CreateAddress]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                      UserId = userId,
                      OrganizationId = organizationId,
                      AddressTypeId = addressTypeId,
                      Name = name,
                      AddressLine1 = addressLine1,
                      AddressLine2 = addressLine2,
                      AddressLine3 = addressLine3,
                      City = city,
                      State = state,
                      Country = country,
                      ZipCode = zipCode,
                      IsDefault = isDefault
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AddressRepository.CreateOrUpdateAddressAsync(), UserId={userId}");
                return -1;
            }
        }

        /// <summary>
        /// Update a address
        /// </summary>        
        /// <param name="addressId"></param>        
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <param name="addressTypeId"></param>
        /// <param name="name"></param>
        /// <param name="addressLine1"></param>
        /// <param name="addressLine2"></param>
        /// <param name="addressLine3"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="country"></param>
        /// <param name="zipCode"></param>
        /// <param name="isDefault"></param>        
        /// <returns>
        /// int
        ///     -1:     Error               
        /// </returns>
        public async Task<int> UpdateAddressAsync(
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
        )
        {
            string procedureName = "[dbo].[UpdateAddress]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        Oid = oid,
                        UserId = userId,
                        OrganizationId = organizationId,
                        AddressTypeId = addressTypeId,
                        Name = name,
                        AddressLine1 = addressLine1,
                        AddressLine2 = addressLine2,
                        AddressLine3 = addressLine3,
                        City = city,
                        State = state,
                        Country = country,
                        ZipCode = zipCode                        
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AddressRepository.UpdateAddressAsync(), UserId={userId}, Oid={oid}");
                return -1;
            }
        }

        /// <summary>
        /// Delete Address
        /// </summary>
        /// <param name="orgainizationId"></param>
        /// <param name="addressId"></param>
        /// <returns>
        /// int
        ///     -1:     Error               
        /// </returns>
        public async Task<int> DeleteAddressAsync(
            int orgainizationId,
            Guid oid
        )
        {
            string procedureName = "[dbo].[DeleteAddress]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = orgainizationId,
                        Oid = oid
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AddressRepository.DeleteAddressAsync(), OrganizationId={orgainizationId}, Oid={oid}");
                return -1;
            }
        }

        /// <summary>
        /// Get all addresss
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Address>> GetAddressesAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetAddresses]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId
                    };
                    return await connection.QueryAsync<Address>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AddressRepository.GetAddresssAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get address
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <returns>Address</returns>
        public async Task<Address> GetAddressAsync(int organizationId, int addressId)
        {
            string procedureName = "[dbo].[GetAddress]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        AddressId = addressId
                    };
                    return await connection.QueryFirstOrDefaultAsync<Address>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AddressRepository.GetAddressAsync(), OrganizationId={organizationId}, AddressId={addressId}");
                return null;
            }
        }

        /// <summary>
        /// Get address
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="oid"></param>
        /// <returns>Address</returns>
        public async Task<Address> GetAddressByOidAsync(int organizationId, Guid oid)
        {
            string procedureName = "[dbo].[GetAddressByOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        Oid = oid
                    };
                    return await connection.QueryFirstOrDefaultAsync<Address>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AddressRepository.GetAddressByOidAsync(), OrganizationId={organizationId}, Oid={oid}");
                return null;
            }
        }
    }
}
