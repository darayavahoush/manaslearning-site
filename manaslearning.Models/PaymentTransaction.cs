using System;

namespace manaslearning.Models
{
    public class PaymentTransaction
    {
        public int PaymentTransactionId { get; set; }
        public Guid Oid { get; set; }        
        public int OrderId { get; set; }
        public string PaymentMode { get; set; }
        public string BankName { get; set; }
        public string BankTransactionId { get; set; }
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TransactionAmount { get; set; }
        public int CurrencyId { get; set; }
        public int PaymentStatusId { get; set; }
        public string GatewayName { get; set; }
        public string GatewayResponse { get; set; }
        public string PaymentTransactionStatus { get; set; }
        public string PaymentTransactionCode { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
}
