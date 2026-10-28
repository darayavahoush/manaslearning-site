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
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public OrganizationRepository(IApplicationConnection applicationConnection, ILogger<OrganizationRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        /// <summary>
        /// Create Organization.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="dispalyName"></param>
        /// <param name="parentOrganizationId"></param>
        /// <param name="organizationTypeId"></param>
        /// <returns>int</returns>
        public async Task<int> CreateOrganizationAsync(long? userId, string name, string dispalyName, int? parentOrganizationId, int organizationTypeId)
        {
            string procedureName = "[dbo].[CreateOrganization]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        Name = name,
                        DisplayName = dispalyName,
                        ParentOrganizationId = parentOrganizationId,
                        OrganizationTypeId = organizationTypeId
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = OrganizationRepository.CreateOrganization, Name={name}, ParentOrganizationId={parentOrganizationId}");
                throw ex;
            }
        }

        /// <summary>
        /// Update Organization.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="dispalyName"></param>
        /// <param name="organizationType"></param>
        /// <param name="parentOrganizationId"></param>
        /// <param name="oid"></param>
        /// <param name="defaultAdmin"></param>
        /// <returns>int</returns>
        public async Task<int> UpdateOrganizationAsync(long? userId, string name, string dispalyName, int organizationType, int? parentOrganizationId, int? defaultAdmin, Guid oid)
        {
            string procedureName = "[dbo].[UpdateOrganization]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        Name = name,
                        DisplayName = dispalyName,
                        OrganizationTypeId = organizationType,
                        ParentOrganizationId = parentOrganizationId,
                        DefaultAdmin = defaultAdmin,
                        Oid = oid
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = OrganizationRepository.UpdateOrganization, Oid={oid}, ParentOrganizationId={parentOrganizationId}");
                return -1;
            }
        }

        /// <summary>
        /// Get Organization by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Organization</returns>
        public async Task<Organization> GetOrganizationByNameAsync(string name)
        {
            string procedureName = "[dbo].[GetOrganizationByName]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<Organization>(procedureName, param: new { Name = name }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = OrganizationRepository.GetOrganizationByNameAsync, Name={name}");
                return null;
            }
        }

        /// <summary>
        /// Get Organization by Oid
        /// </summary>
        /// <param name="oid"></param>
        /// <returns>Organization</returns>
        public async Task<Organization> GetOrganizationByOidAsync(Guid oid)
        {
            string procedureName = "[dbo].[GetOrganizationByOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<Organization>(procedureName, param: new { Oid = oid }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = OrganizationRepository.GetOrganizationByOidAsync, Oid={oid}");
                return null;
            }
        }

        /// <summary>
        /// Get Organization by OrganizationId
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>Organization</returns>
        public async Task<Organization> GetOrganizationAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetOrganization]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<Organization>(procedureName, param: new { OrganizationId = organizationId }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = OrganizationRepository.GetOrganizationAsync, Oid={organizationId}");
                return null;
            }
        }

        /// <summary>
        /// Get Organizations
        /// </summary>        
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Organization>> GetOrganizationsAsync()
        {
            string procedureName = "[dbo].[GetOrganizations]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<Organization>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = OrganizationRepository.GetOrganizationsAsync");
                return null;
            }
        }

        /// <summary>
        /// Get All Organizations
        /// </summary>        
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Organization>> GetAllOrganizationsAsync()
        {
            string procedureName = "[dbo].[GetAllOrganizations]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<Organization>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = OrganizationRepository.GetAllOrganizationsAsync");
                return null;
            }
        }

        public async Task<int> DeleteOrganizationAsync(Guid oid, int userId)
        {
            string procedureName = "[dbo].[DeleteOrganization]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                       Oid = oid,
                       UserId = userId
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = OrganizationRepository.DeleteOrganizationAsync, Oid={oid}");
                return -1;
            }
        }

        public async Task<int> SetOranizationDefaultAdminAsync(int organizationId, long userId, long modifiedBy)
        {
            string procedureName = "[dbo].[SetOrganizationDefaultAdmin]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        UserId = userId,
                        ModifiedBy = modifiedBy
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = OrganizationRepository.SetOranizationDefaultAdminAsync, UserId: {userId}");
                return -1;
            }
        }
    }
}
