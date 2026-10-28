using System;

namespace manaslearning.UI.ViewModels
{
    public class PaymentRequestModel
    {
        public Guid OrderOid { get; set; }        
        public string CurrencyCode { get; set; }
        public string PaymentTransactionStatus { get; set; }        
        public Guid PaymentTransactionOid { get; set; }
    }
}
