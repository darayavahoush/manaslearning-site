using manaslearning.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IOrderStatusRepository
    {
        Task<IEnumerable<OrderStatus>> GetOrderStatusesAsync();
    }
}
