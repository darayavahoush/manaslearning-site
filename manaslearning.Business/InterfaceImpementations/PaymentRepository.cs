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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public PaymentRepository(IApplicationConnection applicationConnection, ILogger<PaymentRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }


        /// <summary>
        /// Create a payment
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderOid"></param>
        /// <param name="currencyCode"></param>        
        /// <param name="paymentTransactionOid"></param>
        /// <returns>
        /// /// int
        ///     -1:     Error 
        /// </returns>
        /// 
        public async Task<int> CreatePaymentAsync(
                long userId,
                Guid orderOid,
                string currencyCode,                
                Guid paymentTransactionOid
            )
        {
            string procedureName = "[dbo].[CreatePayment]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        OrderOid = orderOid,
                        CurrencyCode = currencyCode,                        
                        PaymentTransactionOid = paymentTransactionOid                       
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=PaymentRepository.CreatePaymentAsync(), UserId={userId}, OrderOid={orderOid}, PaymentTransactionOId={paymentTransactionOid}");
                return -1;
            }
        }


        /// <summary>
        /// Update payment
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <param name="paymentTransactionStatus"></param>
        /// <param name="orderOid"></param>
        /// <param name="paymentTransactionOid"></param>
        /// <returns>
        /// /// int
        ///     -1:     Error 
        /// </returns>
        public async Task<int> UpdatePaymentAsync(
               long userId,
               decimal amount,
               string paymentTransactionStatus,               
               Guid paymentTransactionOid
            )
        {
            string procedureName = "[dbo].[UpdatePayment]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        Amount = amount,
                        paymentTransactionStatus = paymentTransactionStatus,                                           
                        PaymentTransactionOid = paymentTransactionOid
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=PaymentRepository.UpdatePaymentAsync(), UserId={userId}, PaymentTransactionOId={paymentTransactionOid}");
                return -1;
            }
        }        

        /// <summary>
        /// Get Payment info
        /// </summary>       
        /// <param name="paymentId"></param>
        /// <returns>PaymentInfo</returns>
        public async Task<PaymentInfo> GetPaymentInfo(int paymentId)
        {
            string procedureName = "[dbo].[GetPaymentInfo]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        PaymentId = paymentId
                    };
                    return await connection.QueryFirstOrDefaultAsync<PaymentInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=PaymentRepository.GetPaymentInfoAsync(), PaymentId={paymentId}");
                return null;
            }
        }

        /// <summary>
        /// Get Payment info by OrderOid
        /// </summary>       
        /// <param name="OrderOid"></param>
        /// <returns>PaymentInfo</returns>
        public async Task<PaymentInfo> GetPaymentInfoByOrderOid(Guid orderOid)
        {
            string procedureName = "[dbo].[GetPaymentInfoByOrderOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrderOid = orderOid
                    };
                    return await connection.QueryFirstOrDefaultAsync<PaymentInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=PaymentRepository.GetPaymentInfoByOrderOidAsync(), OrderOid={orderOid}");
                return null;
            }
        }
    }
}
