using manaslearning.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IPaymentStatusRepository
    {
        Task<IEnumerable<PaymentStatus>> GetPaymentStatusesAsync();
    }
}
