using manaslearning.Models;
using System;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IPaymentTransactionRepository
    {
        Task<int> CreatePaymentTransactionAsync(                
                int orderId
            );

        Task<PaymentTransaction> GetPaymentTransactionAsync(int paymentTransactionId);

        Task<PaymentTransaction> GetPaymentTransactionByOidAsync(Guid oid);

        Task<int> UpdatePaymentTransactionAsync(
            Guid oid,
            long userId,
            string paymentMode,
            string bankName,
            string bankTransactionId,
            string transactionId,
            DateTime transactionDate,
            decimal transactionAmount,
            string currencyCode,
            string paymentTransactionStatus,
            string paymentTransactionCode,
            string gatewayName,
            string gatewayResponse
        );
    }    
}
