using System;

namespace manaslearning.UI.ViewModels
{
    public class UserLicenseDto
    {        
        public Guid AssessmentOid { get; set; }
        public string AssessmentName { get; set; }
        public int AvailableLicenses { get; set; }
        public int UsedLicenses { get; set; }
        public DateTime InitialPurchaseDate { get; set; }
        public DateTime RecentPurchanseDate { get; set; }
    }
}
