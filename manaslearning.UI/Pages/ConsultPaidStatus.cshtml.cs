using manaslearning.Payment;
using manaslearning.Payment.Paytm.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;

namespace manaslearning.UI.Pages
{
    /// <summary>
    /// Paytm callback (POST) and signed return link (GET) for the paid consultation.
    /// The payment is never trusted from the callback itself: the order status is always
    /// fetched from Paytm server-to-server and its signature, order id, merchant id and
    /// amount are checked before the booking link is shown.
    /// </summary>
    [IgnoreAntiforgeryToken] // Paytm posts the callback from its own domain
    public class ConsultPaidStatusModel : PageModel
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IPaytmGateway _paytmGateway;

        public ConsultPaidStatusModel(
            ILogger<ConsultPaidStatusModel> logger,
            IConfiguration configuration,
            IPaytmGateway paytmGateway)
        {
            _logger = logger;
            _configuration = configuration;
            _paytmGateway = paytmGateway;
        }

        public bool Paid { get; private set; }
        public string Message { get; private set; }
        public string ReturnUrl { get; private set; }
        public string OrderId { get; private set; }
        public ConsultationSettings Settings { get; private set; }

        // Signed link a buyer can bookmark to come back and book later.
        public IActionResult OnGet(string orderId, string sig)
        {
            Settings = ConsultationSettings.From(_configuration);
            var paytmConfiguration = LoadPaytmConfiguration();
            if (paytmConfiguration == null)
            {
                Message = "We could not confirm your payment right now. Please try again in a few minutes.";
                return Page();
            }

            if (!ConsultationToken.IsConsultationOrder(orderId) || !ConsultationToken.Verify(orderId, sig, paytmConfiguration.MKey))
            {
                Message = "This link is not valid.";
                return Page();
            }

            VerifyPayment(orderId, paytmConfiguration);
            return Page();
        }

        // Paytm redirects the buyer here after payment, posting the result as form fields.
        public IActionResult OnPost([FromForm(Name = "ORDERID")] string orderId)
        {
            Settings = ConsultationSettings.From(_configuration);
            var paytmConfiguration = LoadPaytmConfiguration();
            if (paytmConfiguration == null)
            {
                Message = "We could not confirm your payment right now. If money was deducted, please contact support with your order id.";
                OrderId = orderId;
                return Page();
            }

            if (!ConsultationToken.IsConsultationOrder(orderId))
            {
                Message = "This payment could not be matched to a consultation.";
                return Page();
            }

            VerifyPayment(orderId, paytmConfiguration);
            return Page();
        }

        private PaytmConfiguration LoadPaytmConfiguration()
        {
            var paytmConfiguration = new PaytmConfiguration();
            _configuration.GetSection("PaymentGateway:Paytm").Bind(paytmConfiguration);
            var validation = paytmConfiguration.IsValid();
            if (!validation.Item1)
            {
                _logger.LogError($"Source=ConsultPaidStatus.LoadPaytmConfiguration(), ErrorMessage={validation.Item2}");
                return null;
            }
            return paytmConfiguration;
        }

        private void VerifyPayment(string orderId, PaytmConfiguration paytmConfiguration)
        {
            string source = $"Source=ConsultPaidStatus.VerifyPayment(), OrderId={orderId}";
            OrderId = orderId;
            try
            {
                _paytmGateway.SetPaytmConfiguration(paytmConfiguration);
                var result = _paytmGateway.GetPaymentResponse(orderId);
                var response = result.Item1;
                bool signatureOk = result.Item2;
                var body = response?.body;

                if (!signatureOk || body == null)
                {
                    _logger.LogError($"{source}, ErrorMessage=Paytm response signature invalid or empty");
                    Message = "We could not confirm your payment. If money was deducted, please contact support with your order id.";
                    return;
                }

                string status = body.resultInfo?.resultStatus;
                if (status == "TXN_SUCCESS")
                {
                    bool orderMatches = string.Equals(body.orderId, orderId, StringComparison.Ordinal);
                    bool merchantMatches = string.Equals(body.mid, paytmConfiguration.MId, StringComparison.Ordinal);
                    bool amountMatches =
                        decimal.TryParse(body.txnAmount, NumberStyles.Number, CultureInfo.InvariantCulture, out var paid)
                        && decimal.TryParse(Settings.PaidAmount, NumberStyles.Number, CultureInfo.InvariantCulture, out var expected)
                        && paid == expected;

                    if (orderMatches && merchantMatches && amountMatches)
                    {
                        Paid = true;
                        string sig = ConsultationToken.Sign(orderId, paytmConfiguration.MKey);
                        ReturnUrl = $"{Request.Scheme}://{Request.Host.Value}/consult/paid/status?orderId={Uri.EscapeDataString(orderId)}&sig={sig}";
                        _logger.LogInformation($"{source}, Info=Paid consultation payment verified");
                        return;
                    }

                    _logger.LogError($"{source}, ErrorMessage=Paytm success did not match expected order, merchant or amount");
                    Message = "We could not confirm your payment. Please contact support with your order id.";
                    return;
                }

                if (status == "PENDING")
                {
                    Message = "Your payment is still being processed. Please refresh this page in a minute.";
                    return;
                }

                Message = "Your payment was not completed, so no booking was made. You have not been charged for this order.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, {source}");
                Message = "We could not confirm your payment right now. If money was deducted, please contact support with your order id.";
            }
        }
    }
}
