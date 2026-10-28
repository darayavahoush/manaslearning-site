namespace manaslearning.T4.Templates.Models
{
    public class EmailVerificationModel
    {
        public string Name { get; set; }
        public string ActivationUrl { get; set; }
        public Company Company { get; set; }
    }
}

