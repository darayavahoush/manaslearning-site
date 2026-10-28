using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.Payment;
using manaslearning.Payment.Paytm.Models;
using manaslearning.Shared;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User
{
    [Authorize]
    public class PaymentStatusModel : PageModel
    {
        private long _userId;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IStorageHelper _storageHelper;
        private readonly IPaytmGateway _paytmGateway;
        private readonly PaytmConfiguration _paytmConfiguration;

        public PaymentStatusModel(
            ILogger<PaymentStatusModel> logger,
            IConfiguration configuration,
            IOrderRepository orderRepository,            
            IStorageHelper storageHelper,
            IPaymentRepository paymentRepository,            
            IPaymentTransactionRepository paymentTransactionRepository,
            IPaytmGateway paytmGateway)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._orderRepository = orderRepository;            
            this._storageHelper = storageHelper;
            this._paymentRepository = paymentRepository;            
            this._paymentTransactionRepository = paymentTransactionRepository;
            this._paytmGateway = paytmGateway;
            this._paytmConfiguration = new PaytmConfiguration();
        }

        public Guid OrderOid { get; set; }

        public string OrderNumber { get; set; }

        public string OrderStatus { get; set; }

        public string PaymentStatus { get; set; }

        public string AmountWithSymbol { get; set; }

        public string TransactionDate { get; set; }

        public Guid AssessmentOid { get; set; }

        public string AssessmentMetaUrl { get; set; }

        public Guid PaymentTransactionOid { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid paymentTransactionOid)
        {
            try
            {
                PaymentTransactionOid = paymentTransactionOid;

                _userId = HttpContext.GetUserId();
                if (_userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                _configuration.GetSection("PaymentGateway:Paytm").Bind(_paytmConfiguration);
                var paytmConfigurationValidation = _paytmConfiguration.IsValid();
                if (!paytmConfigurationValidation.Item1)
                {
                    _logger.LogError(paytmConfigurationValidation.Item2);
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                _paytmGateway.SetPaytmConfiguration(_paytmConfiguration);
                var response = _paytmGateway.GetPaymentResponse(paymentTransactionOid.ToString());                

                if (response.Item1 == null || response.Item1.body == null || response.Item1.body.resultInfo == null)
                {
                    _logger.LogError($"Error=Response is invalid. PaymentTransactionOid = { paymentTransactionOid }");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                if (!response.Item2 && response.Item1.body.resultInfo.resultStatus == "Success")
                {
                    _logger.LogError($"Error=Response Checksum is invalid. PaymentTransactionOid = { paymentTransactionOid }");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                if (string.IsNullOrEmpty(response.Item1.body.txnDate))
                {
                    _logger.LogError($"Error=TxnDate is null. PaymentTransactionOid = { paymentTransactionOid }");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                var transactionDate = Convert.ToDateTime(response.Item1.body.txnDate);
                if (transactionDate == null)
                {
                    _logger.LogError($"Error=TxnDate is invalid. PaymentTransactionOid = { paymentTransactionOid }");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                var updatePaymentTransactionResult = await _paymentTransactionRepository.UpdatePaymentTransactionAsync(
                            paymentTransactionOid,
                            _userId,
                            response.Item1.body.paymentMode,
                            response.Item1.body.bankName,
                            response.Item1.body.bankTxnId,
                            response.Item1.body.txnId,
                            transactionDate.ToUTCTime(StringConstants.INDIAN_TIMEZONE),
                            Convert.ToDecimal(response.Item1.body.txnAmount ?? "0"),
                            "INR",
                            response.Item1.body.resultInfo.resultStatus,
                            response.Item1.body.resultInfo.resultCode,
                            response.Item1.body.gatewayName ?? string.Empty,
                            JsonConvert.SerializeObject(response.Item1, Formatting.Indented)
                            );

                if (updatePaymentTransactionResult <= 0)
                {
                    _logger.LogError($"Error occured while updating the payment transaction for PaymentTransactionOid : { paymentTransactionOid }, ResultCode: {updatePaymentTransactionResult}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                var updatePaymentResult = await _paymentRepository.UpdatePaymentAsync(
                                            _userId,
                                            Convert.ToDecimal(response.Item1.body.txnAmount),
                                            response.Item1.body.resultInfo.resultStatus,
                                            paymentTransactionOid
                                          );

                if (updatePaymentResult <= 0)
                {
                    _logger.LogError($"Error occured while updating the payment for PaymentTransactionOid : { paymentTransactionOid }, ResultCode: {updatePaymentResult}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                var paymentInfo = await _paymentRepository.GetPaymentInfo(updatePaymentResult);
                if (paymentInfo == null)
                {
                    _logger.LogError($"Payment not found for OrderId : {paymentInfo.OrderId}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                var orderInfo = await _orderRepository.GetOrderInfoAsync(_userId, paymentInfo.OrderId);
                if (orderInfo == null)
                {
                    _logger.LogError($"Order not found for OrderId : {paymentInfo.OrderId}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                OrderOid = orderInfo.Oid;
                OrderNumber = orderInfo.OrderNumber;
                OrderStatus = orderInfo.OrderStatusName;
                PaymentStatus = paymentInfo.PaymentStatusName;
                AmountWithSymbol = $"{ orderInfo.CurrencySymbol } { orderInfo.TotalAmount.ToString("F2") }";
                TransactionDate = $"{ transactionDate.ToString(StringConstants.DATE_TIME_FORMAT)} {StringConstants.INDIAN_TIMEZONE_CODE}";                
                AssessmentMetaUrl = orderInfo.AssessmentMetaUrl;
                AssessmentOid = orderInfo.AssessmentOid;

                // Trigger email: Send invoice to user
                if (orderInfo.OrderStatusName == "Completed" && paymentInfo.PaymentStatusName == "Success")
                {
                    var message = GetBase64StringMessage(
                       userId: _userId,
                       orderOid: orderInfo.Oid);

                    var storageAccountName = _configuration.GetValue<string>("StorageAccountName");
                    if (string.IsNullOrWhiteSpace(storageAccountName))
                    {
                        _logger.LogError($"ErrorMessage='StorageAccountName' secret does not exists in keyvault, Source=OrderController.SendInvoiceEmailAsync(), userId={_userId}, orderOid={orderInfo.Oid}");
                        return BadRequest("BadRequest");
                    }

                    var assessmentExcelReportInputQueueName = _configuration.GetValue<string>("Queues:InputQueue:Invoice");
                    if (string.IsNullOrWhiteSpace(assessmentExcelReportInputQueueName))
                    {
                        _logger.LogError($"ErrorMessage='InvoiceInputQueueName' secret does not exists in keyvault, Source=OrderController.SendInvoiceEmailAsync(), userId={_userId}, orderOid={orderInfo.Oid}");
                        return BadRequest("BadRequest");
                    }

                    // Add message to queue
                    var isSuccess = await _storageHelper.InsertMessageIntoQueueAsync(
                        storageAccountName: storageAccountName,
                        queueName: assessmentExcelReportInputQueueName,
                        message: message,
                        logger: _logger);
                }

                return this.Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=User.PaymentStatus.OnGetAsync(), UserId={_userId}, PaymentTransactionOId={paymentTransactionOid}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        /// <summary>
        /// Get Base64 string message
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderOid"></param>
        /// <returns>string</returns>
        private string GetBase64StringMessage(
            long userId,
            Guid orderOid)
        {
            var message = new
            {
                InstanceId = Guid.NewGuid(),
                UserId = userId,
                OrderOid = orderOid,
            };

            var plainText = JsonConvert.SerializeObject(message);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }       
    }
}