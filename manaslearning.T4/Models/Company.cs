namespace manaslearning.T4.Templates.Models
{
    public class Company
    {
        /// <summary>
        /// Company registered name
        /// </summary>
        public string CompanyName { get; set; }
        public string WebsiteName { get; set; }
        public string GSTIN { get; set; }
        public string ContactNo { get; set; }
        public string SupportEmail { get; set; }
        public Address CompanyAddress { get; set; }
        public string TermsUrl { get; set; }
        public string PrivacyUrl { get; set; }
    }
}
