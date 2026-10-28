using Dapper;
using manaslearning.Business.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class LicenseHistoryRepository : ILicenseHistoryRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public LicenseHistoryRepository(IApplicationConnection applicationConnection, ILogger<LicenseHistoryRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
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
            string procedureName = "[dbo].[CreateUserLicenseHistory]";
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
                _logger.LogError(ex, $"{ex.Message}, Source = UserLicenseHistoryRepository.CreateOrganization, UserId={userId}, CreatedBy={createdBy}");
                return -1;
            }
        }
    }
}
