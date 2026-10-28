using System;

namespace manaslearning.UI.ViewModels
{
    public class ItemDto
    {
        public Guid Oid { get; set; }        
        public string ItemDescription { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal GSTRate { get; set; }
        public decimal GSTAmount { get; set; }       
        public decimal Discount { get; set; }
        public decimal MaximumDiscount { get; set; }
        public int AssessmentId { get; set; }
        public string AssessmentCode { get; set; }
        public string AssessmentName { get; set; }
        public string AssessmentDescription { get; set; }
        public bool HasLicense { get; set; }
    }
}
