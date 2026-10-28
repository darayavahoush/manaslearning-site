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
    public class AssessmentReportRepository : IAssessmentReportRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public AssessmentReportRepository(IApplicationConnection applicationConnection, ILogger<AssessmentReportRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        /// <summary>
        /// Get all student assessments
        /// </summary>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<StudentAssessmentInfo>> GetAsync(long userId, Paging paging)
        {
            string procedureName = "[dbo].[GetStudentAssessmentReports]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@UserId", userId, dbType: DbType.Int64);
                    dynamicParameters.Add("@PageSize", paging.PageSize, dbType: DbType.Int32);
                    dynamicParameters.Add("@PageNumber", paging.PageNumber, dbType: DbType.Int32);
                    dynamicParameters.Add("@SortParam", paging.SortColumn, dbType: DbType.Int32);
                    dynamicParameters.Add("@SortDirection", paging.SortDirection, dbType: DbType.Int16);
                    dynamicParameters.Add("@SearchValue", paging.SearchInput, dbType: DbType.String);
                    return await connection.QueryAsync<StudentAssessmentInfo>(procedureName, param: dynamicParameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AssessmentReportRepository.GetAsync(), UserId={userId}");
                return null;
            }
        }

        /// <summary>
        /// Get all student assessments
        /// </summary>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<StudentAssessmentInfo>> GetByOrganizationIdAsync(int organizationId, Paging paging)
        {
            string procedureName = "[dbo].[GetStudentAssessmentReportsByOrganizationId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@OrganizationId", organizationId, dbType: DbType.Int64);
                    dynamicParameters.Add("@PageSize", paging.PageSize, dbType: DbType.Int32);
                    dynamicParameters.Add("@PageNumber", paging.PageNumber, dbType: DbType.Int32);
                    dynamicParameters.Add("@SortParam", paging.SortColumn, dbType: DbType.Int32);
                    dynamicParameters.Add("@SortDirection", paging.SortDirection, dbType: DbType.Int16);
                    dynamicParameters.Add("@SearchValue", paging.SearchInput, dbType: DbType.String);
                    return await connection.QueryAsync<StudentAssessmentInfo>(procedureName, param: dynamicParameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AssessmentReportRepository.GetByOrganizationIdAsync(), OrganizationId={organizationId}");
                return null;
            }
        }

        public async Task<int> GetAssessmentCountByUserAsyc(long userId)
        {
            var storedProcedureName = "[dbo].[GetAssessmentsCountByUser]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@UserId", userId, dbType: DbType.Int64);
                    return await connection.ExecuteScalarAsync<int>(storedProcedureName, param:dynamicParameters,commandType:CommandType.StoredProcedure);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error = {exception.Message}, Source=AssessmentReportRepository.GetCount, UserId={userId}");
                return 0;
            }
        }

        public async Task<int> GetAssessmentCountByOrganizationIdAsyc(int organizationId)
        {
            var storedProcedureName = "[dbo].[GetAssessmentsCountByOrganizationId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@OrganizationId", organizationId, dbType: DbType.Int32);
                    return await connection.ExecuteScalarAsync<int>(storedProcedureName, param: dynamicParameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error = {exception.Message}, Source=AssessmentReportRepository.GetAssessmentCountByOrganizationIdAsyc(), OrganizationId={organizationId}");
                return 0;
            }
        }
    }
}
