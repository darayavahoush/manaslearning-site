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
    public class OrderStatusRepository: IOrderStatusRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public OrderStatusRepository(IApplicationConnection applicationConnection, ILogger<OrderStatusRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderStatus>> GetOrderStatusesAsync()
        {
            string procedureName = "[dbo].[GetOrderStatuses]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                { 
                    return await connection.QueryAsync<OrderStatus>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrderStatusRepository.GetOrderStatusesAsync()");
                return null;
            }
        }
    }
}
