namespace manaslearning.Models
{
    public class Currency
    {
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public bool IsActive { get; set; }
    }
}
