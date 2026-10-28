using System;

namespace manaslearning.Models
{
    public class Order
    {
        public Guid Oid { get; set; }
        public Guid ItemOid { get; set; }
        public string ItemDescription { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public int OrderStatusName { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public int GSTRate { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public string BillingName { get; set; }
        public string BillingAddressLine1 { get; set; }
        public string BillingAddressLine2 { get; set; }
        public string BillingAddressLine3 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingCountry { get; set; }
        public string BillingZipCode { get; set; }
        public string OrderNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
