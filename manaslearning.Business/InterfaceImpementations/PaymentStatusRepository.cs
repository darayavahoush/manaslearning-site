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
    public class PaymentStatusRepository: IPaymentStatusRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public PaymentStatusRepository(IApplicationConnection applicationConnection, ILogger<PaymentStatusRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<PaymentStatus>> GetPaymentStatusesAsync()
        {
            string procedureName = "[dbo].[GetPaymentStatuses]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                { 
                    return await connection.QueryAsync<PaymentStatus>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=PaymentStatusRepository.GetPaymentStatusesAsync()");
                return null;
            }
        }
    }
}
