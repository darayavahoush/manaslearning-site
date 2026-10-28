using System;

namespace manaslearning.Models
{
    public class PaymentInfo
    {
        public Guid Oid { get; set; }
        public int PaymentId { get; set; }
        public int OrderId { get; set; }        
        public int PaymentTransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public int PaymentStatusId { get; set; }
        public string PaymentStatusName { get; set; } 
        
    }
}
