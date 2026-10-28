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
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public AssessmentRepository(IApplicationConnection applicationConnection, ILogger<AssessmentRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        /// <summary>
        /// Get all assessments
        /// </summary>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Assessment>> GetActiveAssessmentsAsync()
        {
            string procedureName = "[dbo].[GetActiveAssessments]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<Assessment>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AssessmentRepository.GetAssessmentsAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get assessment.
        /// </summary>
        /// <param name="id">Assessment.GuidId</param>
        /// <returns>Assessment</returns>
        public async Task<Assessment> GetActiveAssessmentByGuidIdAsync(Guid id)
        {
            string procedureName = "[dbo].[GetActiveAssessmentByGuidId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { @Id = id };
                    return await connection.QueryFirstOrDefaultAsync<Assessment>(
                        procedureName,
                        param: param,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AssessmentRepository.GetAssessmentByGuidIdAsync(), GuidId={id}");
                return null;
            }
        }

        /// <summary>
        /// Get assessment langugages
        /// </summary>
        /// <param name="id">Assessment.GuidId</param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Language>> GetAssessmentLanguagesAsync(Guid id)
        {
            string procedureName = "[dbo].[GetAssessmentLanguages]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { Id = id };
                    return await connection.QueryAsync<Language>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AssessmentRepository.GetAssessmentLanguagesAsync(), Id={id}");
                return null;
            }
        }

        /// <summary>
        /// Get recent 10 assessments
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Tuple of in-progress assessments and recently completed assessments</returns>
        public async Task<Tuple<IEnumerable<RecentAssessment>, IEnumerable<RecentAssessment>>> GetRecentAssessmentsAsync(long userId)
        {
            string procedureName = "[dbo].[GetRecentAssessments]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { UserId = userId };
                    var result = await connection.QueryMultipleAsync(procedureName, param: param, commandType: CommandType.StoredProcedure);

                    var inProgressAssessments = await result.ReadAsync<RecentAssessment>();
                    var recentlyCompletedAssessments = await result.ReadAsync<RecentAssessment>();
                    return new Tuple<IEnumerable<RecentAssessment>, IEnumerable<RecentAssessment>>(inProgressAssessments, recentlyCompletedAssessments);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AssessmentRepository.GetRecentAssessmentsAsync(), UserId={userId}");
                return null;
            }
        }

        /// <summary>
        /// Get recent 10 assessments
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>Tuple of in-progress assessments and recently completed assessments</returns>
        public async Task<Tuple<IEnumerable<RecentAssessment>, IEnumerable<RecentAssessment>>> GetRecentAssessmentsByOrganizationIdAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetRecentAssessmentsByOrganizationId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { OrganizationId = organizationId };
                    var result = await connection.QueryMultipleAsync(procedureName, param: param, commandType: CommandType.StoredProcedure);

                    var inProgressAssessments = await result.ReadAsync<RecentAssessment>();
                    var recentlyCompletedAssessments = await result.ReadAsync<RecentAssessment>();
                    return new Tuple<IEnumerable<RecentAssessment>, IEnumerable<RecentAssessment>>(inProgressAssessments, recentlyCompletedAssessments);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AssessmentRepository.GetRecentAssessmentsByOrganizationIdAsync(), OrganizationId={organizationId}");
                return null;
            }
        }

        /// <summary>
        /// Get all assessments info
        /// </summary>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<AssessmentInfo>> GetAssessmentsInfoAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetAssessmentsInfo]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { @OrganizationId = organizationId };
                    return await connection.QueryAsync<AssessmentInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AssessmentRepository.GetAssessmentsInfoAsync(), OrganizationId={organizationId}");
                return null;
            }
        }

        /// <summary>
        /// Get assessment info
        /// </summary>
        /// <param name="id">Assessment.GuidId</param>
        /// <returns>Assessment</returns>
        public async Task<AssessmentInfo> GetAssessmentInfoByGuidIdAsync(int organizationId, Guid id)
        {
            string procedureName = "[dbo].[GetAssessmentInfoByGuidId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { @Id = id, @OrganizationId = organizationId };
                    return await connection.QueryFirstOrDefaultAsync<AssessmentInfo>(
                        procedureName,
                        param: param,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AssessmentRepository.GetAssessmentInfoByGuidIdAsync(), GuidId={id}, OrganizationId={organizationId}");
                return null;
            }
        }
    }
}
