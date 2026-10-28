using System;

namespace manaslearning.UI.ViewModels
{
    public class OrderRequestModel
    {
        public Guid BillingAddressOid { get; set; }
        public Guid ItemOid { get; set; }
        public int? CurrencyId { get; set; }
        public int Quantity { get; set; }       
    }
}
