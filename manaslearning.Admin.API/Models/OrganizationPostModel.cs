using System.ComponentModel.DataAnnotations;
using System.Text;

namespace manaslearning.Admin.API.Models
{
    public class OrganizationPostModel
    {       
        public int? ParentOrganizationId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(500, ErrorMessage = "Name - Max. 500 characters allowed.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Display Name is required")]
        [StringLength(500, ErrorMessage = "Display Name - Max. 500 characters allowed.")]
        public string DisplayName { get; set; }

        [Range(1, 2, ErrorMessage = "OrganizationType - Invalid")]
        public int OrganizationTypeId { get; set; }

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

            if (!string.IsNullOrEmpty(Name) && string.IsNullOrWhiteSpace(Name?.Trim())) // !string.IsNullOrEmpty(Name) - Added to skip duplicate data annotation error message
            {
                AppendLine("Name is required");
            }

            if (!string.IsNullOrEmpty(DisplayName) && string.IsNullOrWhiteSpace(DisplayName?.Trim()))
            {
                AppendLine("DisplayName is required");
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
