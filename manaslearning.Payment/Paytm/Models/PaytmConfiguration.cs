using System;
using System.Text;

namespace manaslearning.Payment.Paytm.Models
{
    public class PaytmConfiguration
    {
        public string MId { get; set; }
        public string MKey { get; set; }
        public string RequestType { get; set; }
        public string WebsiteName { get; set; }
        public string InitiateTransactionAPI { get; set; }
        public string ShowPaymentPageAPI { get; set; }
        public string OrderStatusAPI { get; set; }

        public Tuple<bool, string> IsValid()
        {            
            bool result = true;
            StringBuilder errorMessages = new StringBuilder();            
            if (string.IsNullOrWhiteSpace(MId))
            {
                AppendLine($"Error='PaymentGateway:Paytm:MID' secret missing in keyvault.");
                result = false;
            }            
            if (string.IsNullOrWhiteSpace(MKey))
            {
                AppendLine($"Error='PaymentGateway:Paytm:MKEY' secret missing in keyvault.");
                result = false;
            }            
            if (string.IsNullOrWhiteSpace(InitiateTransactionAPI))
            {
                AppendLine($"Error='PaymentGateway:Paytm:InitiateTransactionAPI' secret missing in keyvault.");
                result = false;
            }                        
            if (string.IsNullOrWhiteSpace(ShowPaymentPageAPI))
            {
                AppendLine($"Error='PaymentGateway:Paytm:ShowPaymentPageAPI' secret missing in keyvault.");
                result = false;
            }            
            if (string.IsNullOrWhiteSpace(WebsiteName))
            {
                AppendLine($"Error='PaymentGateway:Paytm:WebsiteName' secret missing in keyvault.");
                result = false;
            }            
            if (string.IsNullOrWhiteSpace(RequestType))
            {
                AppendLine($"Error='PaymentGateway:Paytm:RequestType' secret missing in keyvault.");
                result = false;
            }
            return new Tuple<bool, string>(result, errorMessages.ToString());

            void AppendLine(string errorMessage)
            {
                if (errorMessages.Length > 0)
                {
                    errorMessages.AppendLine(errorMessage);
                }
                else { errorMessages.Append(errorMessage); }
            }
        }
    }
}
