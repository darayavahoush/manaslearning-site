namespace manaslearning.T4.Templates.Models
{
    public class ConfirmEmailChangeModel
    {
        public Company Company { get; set; }
        public string Name { get; set; }
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
        public string ConfirmUrl { get; set; }
    }
}
