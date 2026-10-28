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
    public class OrderRepository : IOrderRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public OrderRepository(IApplicationConnection applicationConnection, ILogger<OrderRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        /// <summary>
        /// Create a Order
        /// </summary>        
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <param name="billingAddressOid"></param>
        /// <param name="itemOid"></param>
        /// <param name="quantity"></param>
        /// <param name="orderNumber"></param>        
        /// <returns>
        /// int
        ///     -1:     Error     
        /// </returns>
        public async Task<int> CreateOrderAsync(
            long userId,
            int organizationId,
            Guid billingAddressOid,
            Guid itemOid,
            int currencyId,
            int quantity,
            string orderNumber
        )
        {
            string procedureName = "[dbo].[CreateOrder]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        OrganizationId = organizationId,
                        BillingAddressOid = billingAddressOid,
                        ItemOid = itemOid,
                        Quantity = quantity,
                        CurrencyId = currencyId,
                        OrderNumber = orderNumber
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrderRepository.CreateOrderAsync(), UserId={userId}");
                return -1;
            }
        }
        
        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Order>> GetOrdersAsync(long userId)
        {
            string procedureName = "[dbo].[GetOrders]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId
                    };
                    return await connection.QueryAsync<Order>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrderRepository.GetOrderAsync(), UserId={userId}");
                return null;
            }
        }

        /// <summary>
        /// Get order
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <returns>Order</returns>
        public async Task<Order> GetOrderAsync(long userId, int orderId)
        {
            string procedureName = "[dbo].[GetOrder]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        OrderId = orderId
                    };
                    return await connection.QueryFirstOrDefaultAsync<Order>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrderRepository.GetOrderAsync(), UserId={userId}, OrderId={orderId}");
                return null;
            }
        }

        /// <summary>
        /// Get order by Oid
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oid"></param>
        /// <returns>Order</returns>
        public async Task<Order> GetOrderByOidAsync(long userId, Guid oid)
        {
            string procedureName = "[dbo].[GetOrderByOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        Oid = oid
                    };
                    return await connection.QueryFirstOrDefaultAsync<Order>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrderRepository.GetOrderByOidAsync(), UserId={userId}, Oid={oid}");
                return null;
            }
        }

        /// <summary>
        /// Update Order Status
        /// </summary>         
        /// <param name="userId"></param>
        /// <param name="oid"></param>   
        /// <param name="orderStatusId"></param>             
        /// <returns>
        /// int
        ///     -1:     Error               
        /// </returns>
        public async Task<int> UpdateOrderStatusAsync(            
            long userId,
            Guid oid,
            int orderStatusId
        )
        {
            string procedureName = "[dbo].[UpdateOrderStatus]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        Oid = oid,
                        UserId = userId,
                        OrderStatusId = orderStatusId
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrderRepository.UpdateOrderStatusAsync(), UserId={userId}, Oid={oid}, OrderStatusId={orderStatusId}");
                return -1;
            }
        }

        /// <summary>
        /// Get orderinfo by Oid
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oid"></param>
        /// <returns>OrderInfo</returns>
        public async Task<OrderInfo> GetOrderInfoByOidAsync(long userId, Guid oid)
        {
            string procedureName = "[dbo].[GetOrderInfoByOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        Oid = oid
                    };
                    return await connection.QueryFirstOrDefaultAsync<OrderInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrderRepository.GetOrderInfoByOidAsync(), UserId={userId}, Oid={oid}");
                return null;
            }
        }

        /// <summary>
        /// Get orderinfo
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <returns>OrderInfo</returns>
        public async Task<OrderInfo> GetOrderInfoAsync(long userId, int orderId)
        {
            string procedureName = "[dbo].[GetOrderInfo]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        OrderId = orderId
                    };
                    return await connection.QueryFirstOrDefaultAsync<OrderInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrderRepository.GetOrderInfoAsync(), UserId={userId}, OrderId={orderId}");
                return null;
            }
        }

        /// <summary>
        /// Get ordersinfo
        /// </summary>
        /// <param name="userId"></param>        
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<OrderInfo>> GetOrdersInfoAsync(long userId)
        {
            string procedureName = "[dbo].[GetOrdersInfo]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId
                    };
                    return await connection.QueryAsync<OrderInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=OrderRepository.GetOrdersInfoAsync(), UserId={userId}");
                return null;
            }
        }
    }
}
