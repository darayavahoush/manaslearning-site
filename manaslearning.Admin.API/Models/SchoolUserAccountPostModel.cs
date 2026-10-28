using System.ComponentModel.DataAnnotations;
using System.Text;

namespace manaslearning.Admin.API.Models
{
    public class SchoolUserAccountPostModel
    {
        public class SchoolUser
        {
            [Required(ErrorMessage = "School Name is required")]           
            [StringLength(150, ErrorMessage = "School Name - Max. 150 characters allowed.")]            
            public string SchoolName { get; set; }

            [Required(ErrorMessage = "School Display Name is required")]
            [StringLength(150, ErrorMessage = "School Display Name - Max. 150 characters allowed.")]
            public string SchoolDisplayName { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Email is invalid")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Teacher First Name is required")]           
            [StringLength(50, ErrorMessage = "Teacher First Name - Max. 50 characters allowed.")]
            [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "Teacher First Name is accepts only alphabet and digits")]
            public string TeacherFirstName { get; set; }

            [Required(ErrorMessage = "Teacher Last Name is required")]            
            [StringLength(50, ErrorMessage = "Teacher Last Name - Max. 50 characters allowed.")]
            [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "Teacher Last Name is accepts only alphabet and digits")]
            public string TeacherLastName { get; set; }

            [Required(ErrorMessage = "School Address Line1 is required")]
            [RegularExpression("^[a-zA-Z0-9 -.,/]+$", ErrorMessage = "School Address Line1 - Invalid characters")]
            [StringLength(200, ErrorMessage = "School Address Line1 - Max. 200 characters allowed.")]
            public string SchoolAddressLine1 { get; set; }

            [RegularExpression("^[a-zA-Z0-9 -.,/]+$", ErrorMessage = "School Address Line2 - Invalid characters")]
            [StringLength(200, ErrorMessage = "School Address Line2 - Max. 200 characters allowed.")]
            public string SchoolAddressLine2 { get; set; }

            [RegularExpression("^[a-zA-Z0-9 -.,/]+$", ErrorMessage = "School Address Line3 - Invalid characters")]
            [StringLength(200, ErrorMessage = "School Address Line3 - Max. 200 characters allowed.")]
            public string SchoolAddressLine3 { get; set; }

            [Required(ErrorMessage = "School city is required")]
            [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "School City - Invalid characters")]
            [StringLength(100, ErrorMessage = "School City - Max. 100 characters allowed.")]            
            public string SchoolCity { get; set; }

            [Required(ErrorMessage = "School State is required")]
            [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "School State - Invalid characters")]
            [StringLength(100, ErrorMessage = "School State - Max. 100 characters allowed.")]            
            public string SchoolState { get; set; }

            [Required(ErrorMessage = "School Country is required")]
            [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "School Country - Invalid characters")]
            [StringLength(100, ErrorMessage = "School Country - Max. 100 characters allowed.")]
            public string SchoolCountry { get; set; }

            [Required(ErrorMessage = "School Zipcode is required")]            
            [StringLength(10, ErrorMessage = "School Zipcode - Max. 10 characters allowed.")]
            public string SchoolZipCode { get; set; }

            [StringLength(50, ErrorMessage = "Max. 50 characters allowed.")]
            public string SchoolPhone1 { get; set; }

            [StringLength(50, ErrorMessage = "Max. 50 characters allowed.")]
            public string SchoolPhone2 { get; set; }
            public IEnumerable<SchoolUserLicense> Licenses { get; set; }


            public Tuple<bool, string> IsValid()
            {
                bool result = true;
                ICollection<ValidationResult> validationErrors = new List<ValidationResult>();
                StringBuilder errorMessages = new StringBuilder();
               
                Validator.TryValidateObject(this
                , new ValidationContext(this)
                , validationErrors, true);

                if (validationErrors.Count > 0)
                {
                    result = false;
                    foreach (var validationError in validationErrors)
                    {
                        AppendLine(validationError.ErrorMessage);
                    }
                }

                if (!string.IsNullOrEmpty(SchoolName) && string.IsNullOrWhiteSpace(SchoolName?.Trim())) // !string.IsNullOrEmpty(SchoolName) - Added to skip duplicate data annotation error message
                {
                    AppendLine("School Name is required");
                }

                if (!string.IsNullOrEmpty(SchoolDisplayName) && string.IsNullOrWhiteSpace(SchoolDisplayName?.Trim()))
                {
                    AppendLine("School Display Name is required");
                }

                if (!string.IsNullOrEmpty(TeacherFirstName) && string.IsNullOrWhiteSpace(TeacherFirstName?.Trim()))
                {
                    AppendLine("Teacher First Name is required");
                }

                if (!string.IsNullOrEmpty(TeacherLastName) && string.IsNullOrWhiteSpace(TeacherLastName?.Trim()))
                {
                    AppendLine("Teacher Last Name is required");
                }

                if (!string.IsNullOrEmpty(SchoolAddressLine1) && string.IsNullOrWhiteSpace(SchoolAddressLine1?.Trim()))
                {
                    AppendLine("School Address Line1 is required");
                }

                if (!string.IsNullOrEmpty(SchoolCity) && string.IsNullOrWhiteSpace(SchoolCity?.Trim()))
                {
                    AppendLine("School City is required");
                }

                if (!string.IsNullOrEmpty(SchoolState) && string.IsNullOrWhiteSpace(SchoolState?.Trim()))
                {
                    AppendLine("School State is required");
                }

                if (!string.IsNullOrEmpty(SchoolCountry) && string.IsNullOrWhiteSpace(SchoolCountry?.Trim()))
                {
                    AppendLine("School Country is required");
                }

                if (!string.IsNullOrEmpty(SchoolZipCode) && string.IsNullOrWhiteSpace(SchoolZipCode?.Trim()))
                {
                    AppendLine("School ZipCode is required");
                }

                return new Tuple<bool, string>(result, errorMessages.ToString());
               
                void AppendLine(string errorMessage)
                {
                    if (errorMessages.Length > 0)
                    {
                        errorMessages.AppendLine(errorMessage);
                    }
                    else { errorMessages.Append(errorMessage); }
                }
            }
        }

        public class SchoolUserLicense
        {
            public string AssessmentCode { get; set; }
            public int LicenseCount { get; set; }
        }
    }
}
