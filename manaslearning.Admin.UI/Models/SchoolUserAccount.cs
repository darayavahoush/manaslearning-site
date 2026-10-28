using System.Collections.Generic;

namespace manaslearning.Admin.UI.Models
{
    public class SchoolUserAccount
    {
        public string SchoolName { get; set; }
        public string SchoolDisplayName { get; set; }
        public string Email { get; set; }
        public string TeacherFirstName { get; set; }
        public string TeacherLastName { get; set; }
        public string SchoolAddressLine1 { get; set; }
        public string SchoolAddressLine2 { get; set; }
        public string SchoolAddressLine3 { get; set; }
        public string SchoolCity { get; set; }
        public string SchoolState { get; set; }
        public string SchoolCountry { get; set; }
        public string SchoolZipCode { get; set; }
        public string SchoolPhone1 { get; set; }
        public string SchoolPhone2 { get; set; }
        public List<SchoolUserLicense> Licenses { get; set; }
    }

    public class SchoolUserLicense
    {
        public string AssessmentCode { get; set; }
        public int LicenseCount { get; set; }
    }
}
