namespace manaslearning.Models
{
    public class PaymentStatus
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusDescription { get; set; }
        public bool IsActive { get; set; }
    }
}
