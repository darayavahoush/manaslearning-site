using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public PaymentTransactionRepository(IApplicationConnection applicationConnection, ILogger<PaymentTransactionRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        
        /// <summary>
        /// Create a payment transaction
        /// </summary>        
        /// <param name="orderId"></param>        
        /// <returns>
        /// /// int
        ///     -1:     Error 
        /// </returns>
        public async Task<int> CreatePaymentTransactionAsync(
                int orderId
        )
        {
            string procedureName = "[dbo].[CreatePaymentTransaction]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrderId = orderId                                             
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=PaymentTransactionRepository.CreatePaymentTransactionAsync(), OrderId={orderId}");
                return -1;
            }
        }

        /// <summary>
        /// Get Payment Transaction
        /// </summary>        
        /// <param name="paymentTransactionId"></param>
        /// <returns>PaymentTransaction</returns>
        public async Task<PaymentTransaction> GetPaymentTransactionAsync(int paymentTransactionId)
        {
            string procedureName = "[dbo].[GetPaymentTransaction]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        PaymentTransactionId = paymentTransactionId
                    };
                    return await connection.QueryFirstOrDefaultAsync<PaymentTransaction>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=PaymentTransactionRepository.GetPaymentTransactionAsync(), PaymentTransactionId={paymentTransactionId}");
                return null;
            }
        }

        /// <summary>
        /// Get paymentTransaction by Oid
        /// </summary>        
        /// <param name="oid"></param>
        /// <returns>PaymentTransaction</returns>
        public async Task<PaymentTransaction> GetPaymentTransactionByOidAsync(Guid oid)
        {
            string procedureName = "[dbo].[GetPaymentTransactionByOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        Oid = oid
                    };
                    return await connection.QueryFirstOrDefaultAsync<PaymentTransaction>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=PaymentTransactionRepository.GetPaymentTransactionByOidAsync(), Oid={oid}");
                return null;
            }
        }


        /// <summary>
        /// Update a payment transaction
        /// </summary>  
        /// <param name="oid"></param>
        /// <param name="userId"></param>
        /// <param name="paymentMode"></param>
        /// <param name="bankName"></param>
        /// <param name="bankTransactionId"></param>
        /// <param name="transactionId"></param>
        /// <param name="transactionDate"></param>
        /// <param name="transactionAmount"></param>
        /// <param name="currency"></param>
        /// <param name="paymentTransactionStatus"></param>
        /// <param name="paymentTransactionCode"></param>
        /// <param name="gatewayName"></param>
        /// <param name="gatewayResponse"></param>
        /// <returns>
        /// /// int
        ///     -1:     Error 
        /// </returns>
        public async Task<int> UpdatePaymentTransactionAsync(
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
        )
        {
            string procedureName = "[dbo].[UpdatePaymentTransaction]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        Oid = oid,
                        UserId = userId,
                        PaymentMode = paymentMode,
                        BankName = bankName,
                        BankTransactionId = bankTransactionId,
                        TransactionId = transactionId,
                        TransactionDate = transactionDate,
                        TransactionAmount = transactionAmount,
                        CurrencyCode = currencyCode,
                        PaymentTransactionStatus = paymentTransactionStatus,
                        PaymentTransactionCode = paymentTransactionCode,
                        GatewayName = gatewayName,
                        GatewayResponse = gatewayResponse
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AddressRepository.UpdatePaymentTransactionAsync(), UserId={userId}, Oid={oid}");
                return -1;
            }
        }
    }
}
