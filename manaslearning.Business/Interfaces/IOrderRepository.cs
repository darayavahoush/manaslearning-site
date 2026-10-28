using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderAsync(
                long userId,
                int organizationId,
                Guid billingAddressOid,
                Guid itemOid,
                int currencyId,
                int quantity,
                string orderNumber
            );

        Task<IEnumerable<Order>> GetOrdersAsync(long userId);

        Task<Order> GetOrderAsync(long userId, int orderId);

        Task<Order> GetOrderByOidAsync(long userId, Guid oid);

        Task<int> UpdateOrderStatusAsync(long userId, Guid oid, int orderStatusId);

        Task<IEnumerable<OrderInfo>> GetOrdersInfoAsync(long userId);

        Task<OrderInfo> GetOrderInfoByOidAsync(long userId, Guid oid);

        Task<OrderInfo> GetOrderInfoAsync(long userId, int orderId);

    }
}
