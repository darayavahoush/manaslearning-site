using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class QualificationRepository : IQualificationRepository
    {
        private readonly ILogger _logger;
        private readonly IApplicationConnection _applicationConnection;

        public QualificationRepository(ILogger<QualificationRepository> logger, IApplicationConnection applicationConnection)
        {
            _logger = logger;
            _applicationConnection = applicationConnection;
        }

        public async Task<IEnumerable<Qualification>> GetQualificationsAsync()
        {
            var storedProcedureName = "[dbo].[GetQualifications]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<Qualification>(storedProcedureName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message},Source=QualificationRepository.GetQualificationsAsync()");
                return null;
            }
        }
    }
}
