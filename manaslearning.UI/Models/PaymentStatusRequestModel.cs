using System;

namespace manaslearning.UI.ViewModels
{
    public class PaymentStatusRequestModel
    {
        public Guid PaymentOid { get; set; }
        public int PaymentStatusId { get; set; }
    }
}
