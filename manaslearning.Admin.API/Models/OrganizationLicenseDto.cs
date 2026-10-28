namespace manaslearning.Admin.API.Models
{
    public class OrganizationLicenseDto
    {
        public int LicenseId { get; set; }
        public int OrganizationId { get; set; }
        public int AssessmentId { get; set; }
        public Guid AssessmentOid { get; set; }
        public string AssessmentName { get; set; }
        public int AvailableLicenses { get; set; }
        public int UsedLicenses { get; set; }
        public DateTime InitialPurchaseDate { get; set; }
        public DateTime RecentPurchanseDate { get; set; }
    }
}
