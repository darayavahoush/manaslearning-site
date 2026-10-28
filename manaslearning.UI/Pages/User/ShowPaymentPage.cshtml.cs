using manaslearning.Business.Interfaces;
using manaslearning.Payment;
using manaslearning.Payment.Paytm.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User
{
    [Authorize]
    public class ShowPaymentPage : PageModel
    {
        private long _userId;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PaytmConfiguration _paytmConfiguration;
        private readonly IPaytmGateway _paytmGateway;


        public ShowPaymentPage(
            ILogger<ShowPaymentPage> logger,
            IConfiguration configuration,
            IPaymentTransactionRepository paymentTransactionRepository,
            IPaymentRepository paymentRepository,            
            IOrderRepository orderRepository,
            IHttpContextAccessor httpContextAccessor,
            IPaytmGateway paytmGateway
            )
        {
            _logger = logger;            
            this._configuration = configuration;            
            this._paymentTransactionRepository = paymentTransactionRepository;
            this._paymentRepository = paymentRepository;
            this._orderRepository = orderRepository;
            this._httpContextAccessor = httpContextAccessor;           
            this._paytmConfiguration = new PaytmConfiguration();
            this._paytmGateway = paytmGateway;
        }

        public string Mid { get; set; }
        public string OrderId { get; set; }
        public string PaymentUrl { get; set; }
        public string TxnToken { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid orderOid)
        {
            string errorSourceWithInputParams = $"Source=ShowPaymentPage.OnGetAsync(), OrderOid={orderOid}";
            try
            {
                _userId = HttpContext.GetUserId();
                if (_userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                var orderInfo = await _orderRepository.GetOrderInfoByOidAsync(_userId, orderOid);
                if (orderInfo == null)
                {
                    _logger.LogError($"Order not found for OrderOid : {orderOid}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                if (orderInfo.OrderStatusName != "Pending")
                {
                    _logger.LogError($"Order should be in Pending status. OrderOid : {orderOid}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                string currency = "INR"; // HACK: for now INR is the currency
                string callbackUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}/api/u/paymentstatus";

                _configuration.GetSection("PaymentGateway:Paytm").Bind(_paytmConfiguration);
                var paytmConfigurationValidation = _paytmConfiguration.IsValid();
                if (!paytmConfigurationValidation.Item1)
                {
                    _logger.LogError(paytmConfigurationValidation.Item2);
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                var paymentTransactionResult = await _paymentTransactionRepository.CreatePaymentTransactionAsync(
                       orderInfo.OrderId
                    );

                if (paymentTransactionResult == -1) // -1: ERROR
                {
                    _logger.LogError($"Error=While Creating PaymentTransaction. {errorSourceWithInputParams}");                    
                    return this.Page();
                }
                var paymentTransaction = await _paymentTransactionRepository.GetPaymentTransactionAsync(paymentTransactionResult);
                if (paymentTransaction == null)
                {
                    _logger.LogError($"Error=PaymentTransaction can not be null. {errorSourceWithInputParams}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                var payment = await _paymentRepository.GetPaymentInfoByOrderOid(orderInfo.Oid);
                if (payment == null)
                {
                    var paymentResult = await _paymentRepository.CreatePaymentAsync(
                                        _userId,
                                        orderInfo.Oid,
                                        currency,
                                        paymentTransaction.Oid
                                    );

                    if (paymentResult <= 0)
                    {
                        _logger.LogError($"Error occured while creating Payment. ErrorCode={paymentResult}. {errorSourceWithInputParams}");
                        return LocalRedirect(UrlConstants.USER_ERROR);
                    }
                }
                
                decimal totalAmount = Math.Round(orderInfo.TotalAmount, 2) + 0.00m;
                _paytmGateway.SetPaytmConfiguration(_paytmConfiguration);
                string txnToken = _paytmGateway.InitiateTransactionAPI(paymentTransaction.Oid.ToString(), currency, totalAmount.ToString(), callbackUrl, _userId);
                string paymentPageUrl = $"{_paytmConfiguration.ShowPaymentPageAPI}?mid={_paytmConfiguration.MId}&orderId={paymentTransaction.Oid}";
                this.Mid = _paytmConfiguration.MId;
                this.OrderId =paymentTransaction.Oid.ToString();
                this.PaymentUrl = paymentPageUrl;
                this.TxnToken = txnToken;
                return this.Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, {errorSourceWithInputParams}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }            
        }
    }   
}