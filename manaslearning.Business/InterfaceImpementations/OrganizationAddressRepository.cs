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
    public class OrganizationAddressRepository : IOrganizationAddressRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public OrganizationAddressRepository(IApplicationConnection applicationConnection, ILogger<OrganizationAddressRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        /// <summary>
        /// Create a Organization address
        /// </summary>        
        /// <param name="organizationId"></param>        
        /// <param name="addressTypeId"></param>        
        /// <param name="addressLine1"></param>
        /// <param name="addressLine2"></param>
        /// <param name="addressLine3"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="country"></param>
        /// <param name="zipCode"></param>
        /// <param name="phone1"></param>
        /// <param name="phone2"></param>
        /// <param name="isDefault"></param>        
        /// <returns>
        /// int
        ///     -1:     Error     
        /// </returns>
        public async Task<int> CreateOrganizationAddressAsync(            
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
        )
        {
            string procedureName = "[dbo].[CreateOrganizationAddress]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                      OrganizationId = organizationId,
                      AddressTypeId = addressTypeId,                      
                      AddressLine1 = addressLine1,
                      AddressLine2 = addressLine2,
                      AddressLine3 = addressLine3,
                      City = city,
                      State = state,
                      Country = country,
                      ZipCode = zipCode,
                      Phone1 = phone1,
                      Phone2 = phone2,
                      IsDefault = isDefault
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrganizationAddressRepository.CreateOrganizationAddressAsync(), OrganizationId={organizationId}");
                return -1;
            }
        }

        /// <summary>
        /// Update Organization address
        /// </summary> 
        /// <param name="addressId"></param>  
        /// <param name="organizationId"></param>
        /// <param name="addressTypeId"></param>        
        /// <param name="addressLine1"></param>
        /// <param name="addressLine2"></param>
        /// <param name="addressLine3"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="country"></param>
        /// <param name="zipCode"></param>
        /// <param name="phone1"></param>
        /// <param name="phone2"></param>
        /// <param name="isDefault"></param>        
        /// <returns>
        /// int
        ///     -1:     Error     
        /// </returns>
        public async Task<int> UpdateOrganizationAddressAsync(
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
        )
        {
            string procedureName = "[dbo].[UpdateOrganizationAddress]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        AddressId = addressId,
                        OrganizationId = organizationId,
                        AddressTypeId = addressTypeId,
                        AddressLine1 = addressLine1,
                        AddressLine2 = addressLine2,
                        AddressLine3 = addressLine3,
                        City = city,
                        State = state,
                        Country = country,
                        ZipCode = zipCode,
                        Phone1 = phone1,
                        Phone2 = phone2,
                        IsDefault = isDefault
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrganizationAddressRepository.UpdateOrganizationAddressAsync(), Addressid={addressId}");
                return -1;
            }
        }

        /// <summary>
        /// Get all addresss by OrganizationId
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<OrganizationAddress>> GetOrganizationAddressesAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetOrganizationAddresses]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId
                    };
                    return await connection.QueryAsync<OrganizationAddress>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrganizationAddressRepository.GetOrganizationAddresssAsync(), OrganizationId={organizationId}");
                return null;
            }
        }

        /// <summary>
        /// Get All Organization Addresses by Organization Oid
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <returns></returns>
        public async Task<IEnumerable<OrganizationAddress>> GetAllOrganizationAddressesByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetAllOrganizationAddressesByOrganizationOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid
                    };
                    return await connection.QueryAsync<OrganizationAddress>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrganizationAddressRepository.GetAllAddressesByOrganizationOidAsync(), OrganizationOid={organizationOid}");
                return null;
            }
        }

        public async Task<IEnumerable<OrganizationAddress>> GetOrganizationAddressesByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetOrganizationAddressesByOrganizationOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid
                    };
                    return await connection.QueryAsync<OrganizationAddress>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrganizationAddressRepository.GetAddressesByOrganizationOidAsync(), OrganizationOid={organizationOid}");
                return null;
            }
        }

        /// <summary>
        /// Get Organization Address
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <param name="organizationAddressOid"></param>
        /// <returns></returns>
        public async Task<OrganizationAddress> GetOrganizationAddressAsync(Guid organizationOid, Guid organizationAddressOid)
        {
            string procedureName = "[dbo].[GetOrganizationAddress]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid,
                        OrganizationAddressOid = organizationAddressOid
                    };
                    return await connection.QueryFirstOrDefaultAsync<OrganizationAddress>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrganizationAddressRepository.GetOrganizationAddresses(), OrganizationOid={organizationOid}");
                return null;
            }
        }

        /// <summary>
        /// Delete Organization Address
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public async Task<int> DeleteOrganizationAddressAsync(Guid oid)
        {
            string procedureName = "[dbo].[DeleteOrganizationAddress]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        Oid = oid
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = OrganizationAddressRepository.DeleteOrganizationAddressAsync, Oid={oid}");
                return -1;
            }
        }
    }
}
