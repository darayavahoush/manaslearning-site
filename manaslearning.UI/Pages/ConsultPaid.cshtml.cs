using manaslearning.Payment;
using manaslearning.Payment.Paytm.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace manaslearning.UI.Pages
{
    /// <summary>
    /// Starts a Paytm payment for the paid consultation. No login and no database: the booking
    /// calendar link is only shown by ConsultPaidStatus after Paytm confirms the payment.
    /// </summary>
    public class ConsultPaidModel : PageModel
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IPaytmGateway _paytmGateway;

        public ConsultPaidModel(
            ILogger<ConsultPaidModel> logger,
            IConfiguration configuration,
            IPaytmGateway paytmGateway)
        {
            _logger = logger;
            _configuration = configuration;
            _paytmGateway = paytmGateway;
        }

        public string Mid { get; set; }
        public string OrderId { get; set; }
        public string PaymentUrl { get; set; }
        public string TxnToken { get; set; }
        public string Error { get; set; }
        public ConsultationSettings Settings { get; private set; }

        public void OnGet()
        {
            Settings = ConsultationSettings.From(_configuration);
        }

        public IActionResult OnPost()
        {
            Settings = ConsultationSettings.From(_configuration);
            string orderId = ConsultationToken.NewOrderId();
            string source = $"Source=ConsultPaid.OnPost(), OrderId={orderId}";
            try
            {
                var paytmConfiguration = new PaytmConfiguration();
                _configuration.GetSection("PaymentGateway:Paytm").Bind(paytmConfiguration);
                var validation = paytmConfiguration.IsValid();
                if (!validation.Item1)
                {
                    _logger.LogError($"{source}, ErrorMessage={validation.Item2}");
                    Error = "Online payment is not available right now. Please try again later or contact support.";
                    return Page();
                }

                string callbackUrl = $"{Request.Scheme}://{Request.Host.Value}/consult/paid/status";

                _paytmGateway.SetPaytmConfiguration(paytmConfiguration);
                // The amount always comes from server settings, never from the request.
                // custId 0: the buyer is anonymous (no login on this flow).
                string txnToken = _paytmGateway.InitiateTransactionAPI(orderId, "INR", Settings.PaidAmount, callbackUrl, 0);

                Mid = paytmConfiguration.MId;
                OrderId = orderId;
                TxnToken = txnToken;
                PaymentUrl = $"{paytmConfiguration.ShowPaymentPageAPI}?mid={paytmConfiguration.MId}&orderId={orderId}";
                _logger.LogInformation($"{source}, Info=Paid consultation payment started");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, {source}");
                Error = "We could not start the payment. Please try again.";
                return Page();
            }
        }
    }
}
