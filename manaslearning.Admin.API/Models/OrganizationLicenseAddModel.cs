using System.ComponentModel.DataAnnotations;
using System.Text;

namespace manaslearning.Admin.API.Models
{
    public class OrganizationLicenseAddModel
    {
        [Required(ErrorMessage = "OrganizationOid is required")]
        public Guid OrganizationOid { get; set; }

        [Required(ErrorMessage = "AssessmentOid is required")]
        public Guid AssessmentOid { get; set; }
       
        public Guid? ItemOid { get; set; }

        [Required(ErrorMessage = "Licenses is required")]
        public int Licenses { get; set; }

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

            if (Licenses <= 0)
            {
                AppendLine("Licenses should be greater than zero.");
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
