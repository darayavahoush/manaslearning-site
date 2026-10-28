using manaslearning.Models;
using System;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IPaymentRepository
    {
        Task<int> CreatePaymentAsync(
                long userId,
                Guid orderOid,
                string currencyCode,                
                Guid paymentTransactionOid
            );

        Task<int> UpdatePaymentAsync(
               long userId,
               decimal amount,
               string paymentTransactionStatus,               
               Guid paymentTransactionOid
            );

        Task<PaymentInfo> GetPaymentInfoByOrderOid(Guid orderOid);

        Task<PaymentInfo> GetPaymentInfo(int paymentId);
    }    
}
