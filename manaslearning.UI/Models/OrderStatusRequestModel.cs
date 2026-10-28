using System;

namespace manaslearning.UI.ViewModels
{
    public class OrderStatusRequestModel
    {
        public Guid OrderOid { get; set; }
        public int OrderStatusId { get; set; }
    }
}
