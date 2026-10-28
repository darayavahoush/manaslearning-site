using System.ComponentModel.DataAnnotations;
using System.Text;

namespace manaslearning.Admin.API.Models
{
    public class OrganizationStudentPutModel
    {  
        public Guid StudentOid { get; set; }
        [Required(ErrorMessage = "OrganizationOid is required")]
        public Guid OrganizationOid { get; set; }

        [Required(ErrorMessage = "StudentName is required")]
        [StringLength(50, ErrorMessage = "StudentName - Max. 50 characters allowed.")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "ParentName is required")]
        [StringLength(50, ErrorMessage = "ParentName - Max. 50 characters allowed.")]
        public string ParentName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(512, ErrorMessage = "Email - Max. 512 characters allowed.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [StringLength(15, ErrorMessage = "PhoneNumber - Max. 15 characters allowed.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Zip is required")]
        [StringLength(15, ErrorMessage = "Zip - Max. 10 characters allowed.")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "LanguagesCanSpeak is required")]
        [StringLength(1000, ErrorMessage = "LanguagesCanSpeak - Max. 1000 characters allowed.")]
        public string LanguagesCanSpeak { get; set; }

        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateTime DateOfBirth { get; set; }

        public int StudentClass { get; set; }
        public int ParentEducation { get; set; }

        [Required(ErrorMessage = "FirstLanguage is required")]        
        public int FirstLanguage { get; set; }

        [Required(ErrorMessage = "SecondLanguage is required")]        
        public int SecondLanguage { get; set; }
        
        public int ThirdLanguage { get; set; }

        [Required(ErrorMessage = "MotherTongue is required")]
        public int MotherTongue { get; set; }

        [Required(ErrorMessage = "GenderId is required")]
        public int GenderId { get; set; }

        [Required(ErrorMessage = "SchoolTypeId is required")]
        public int SchoolTypeId { get; set; }

        [Required(ErrorMessage = "SyllabusTypeId is required")]
        public int SyllabusTypeId { get; set; }
        public int SectionId { get; set; }
        public bool IsActive { get; set; }

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
                    AppendLine(validationError?.ErrorMessage ?? string.Empty);
                }
            }

            if (!string.IsNullOrEmpty(StudentName) && string.IsNullOrWhiteSpace(StudentName?.Trim())) // !string.IsNullOrEmpty(StudentName) - Added to skip duplicate data annotation error message
            {
                AppendLine("StudentName is required");
            }

            if (!string.IsNullOrEmpty(ParentName) && string.IsNullOrWhiteSpace(ParentName?.Trim()))
            {
                AppendLine("ParentName is required");
            }

            if (!string.IsNullOrEmpty(Email) && string.IsNullOrWhiteSpace(Email?.Trim()))
            {
                AppendLine("Email is required");
            }

            if (!string.IsNullOrEmpty(PhoneNumber) && string.IsNullOrWhiteSpace(PhoneNumber?.Trim()))
            {
                AppendLine("ZipCode is required");
            }

            if (!string.IsNullOrEmpty(Zip) && string.IsNullOrWhiteSpace(Zip?.Trim()))
            {
                AppendLine("ZipCode is required");
            }

            if (!string.IsNullOrEmpty(LanguagesCanSpeak) && string.IsNullOrWhiteSpace(LanguagesCanSpeak?.Trim()))
            {
                AppendLine("LanguagesCanSpeak is required");
            }

            if (FirstLanguage <= 0)
            {
                AppendLine("Firstlanguage is required");
            }

            if (SecondLanguage <= 0)
            {
                AppendLine("SecondLanguage is required");
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
}
