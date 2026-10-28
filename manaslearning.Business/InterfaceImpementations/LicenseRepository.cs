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
    public class LicenseRepository : ILicenseRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public LicenseRepository(IApplicationConnection applicationConnection, ILogger<LicenseRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        /// <summary>
        /// Get all user licenses
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<License>> GetOrganizationLicensesAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetOrganizationLicenses]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId
                    };
                    return await connection.QueryAsync<License>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=LicenseRepository.GetOrganizationLicensesAsync(), OrganizationId={organizationId}");
                return null;
            }
        }

        public async Task<License> GetOrganizationLicenseByAssessmentIdAsync(int organizationId, int assessmentId)
        {
            string procedureName = "[dbo].[GetOrganizationLicenseByAssessmentId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        AssessmentId = assessmentId
                    };
                    return await connection.QueryFirstOrDefaultAsync<License>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=LicenseRepository.GetOrganizationLicenseByAssessmentIdAsync(), OrganizationId={organizationId}, AssessmentId={assessmentId}");
                return null;
            }
        }

        /// <summary>
        /// Get all organization licenses info
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<LicenseInfo>> GetOrganizationLicensesInfoAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetOrganizationLicensesInfo]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId
                    };
                    return await connection.QueryAsync<LicenseInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=LicenseRepository.GetOrganizationLicensesInfo(), OrganizationId={organizationId}");
                return null;
            }
        }

        /// <summary>
        /// Get all organization licenses info
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<LicenseInfo>> GetLicensesInfoByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetLicensesInfoByOrganizationOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid
                    };
                    return await connection.QueryAsync<LicenseInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=LicenseRepository.GetLicensesInfoByOrganizationOidAsync(), OrganizationOid={organizationOid}");
                return null;
            }
        }

        /// <summary>
        /// Create/Update User License.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="assessmentId"></param>
        /// <param name="itemId"></param>
        /// <param name="licenses"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public async Task<int> CreateLicenseHistoryAsync(long userId, int assessmentId, int itemId, int licenses, long createdBy)
        {
            string procedureName = "[dbo].[CreateOrganizationLicense]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        AssessmentId = assessmentId,
                        ItemId = itemId,
                        CreatedBy = createdBy,
                        Licenses = licenses
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = LicenseHistoryRepository.CreateLicenseHistoryAsync, UserId={userId}, CreatedBy={createdBy}");
                return -1;
            }
        }

        public async Task<int>ReduceAvailableLicenseByOne(int organizationId, int assessmentId)
        {
            string procedureName = "[dbo].[ReduceAvailableLicenseByOne]";

            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        AssessmentId = assessmentId
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=LicenseRepository.ReduceAvailableLicenseByOne(), OrganizationId={organizationId}, AssessmentId={assessmentId}");
                return -1;
            }
        }

        public async Task<int> AddOrganizationLicensesAsync(int organizationId, int assessmentId, int itemId, int licenses, long createdBy)
        {
            string procedureName = "[dbo].[AddOrganizationLicenses]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        AssessmentId = assessmentId,
                        ItemId = itemId,
                        CreatedBy = createdBy,
                        Licenses = licenses
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = LicenseRepository.AddOrganizationLicensesAsync, OrganizationId={organizationId}, AssessmentId={assessmentId}, CreatedBy={createdBy}");
                return -1;
            }
        }

        public async Task<int> ReduceOrganizationLicensesAsync(int organizationId, int assessmentId, int licenses, long createdBy)
        {
            string procedureName = "[dbo].[ReduceOrganizationLicenses]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        AssessmentId = assessmentId,
                        CreatedBy = createdBy,
                        Licenses = licenses
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = LicenseRepository.ReduceOrganizationLicensesAsync, OrganizationId={organizationId}, AssessmentId={assessmentId}, CreatedBy={createdBy}");
                return -1;
            }
        }
    }
}
