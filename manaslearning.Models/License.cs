using System;

namespace manaslearning.Models
{
    public class License
    {
        public int LicenseId { get; set; }
        public int OrganizationId { get; set; }
        public int AssessmentId { get; set; }
        public int AvailableLicenses { get; set; }
        public int UsedLicenses { get; set;  }
        public DateTime InitialPurchaseDate { get; set; }
        public DateTime RecentPurchanseDate { get; set; }
    }
}

