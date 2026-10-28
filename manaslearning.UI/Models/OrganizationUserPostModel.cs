using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace manaslearning.UI.ViewModels
{
    public class OrganizationUserPostModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name - Max. 50 characters allowed.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name - Max. 50 characters allowed.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Display Name is required")]
        [StringLength(100, ErrorMessage = "Display Name - Max. 100 characters allowed.")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(256, ErrorMessage = "Email - Max. 256 characters allowed.")]
        [EmailAddress]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        [StringLength(15, ErrorMessage = "PhoneNumber - Max. 15 characters allowed.")]
        public string PhoneNumber { get; set; }
        
        public string ClassIds { get; set; }
         

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

            if (!string.IsNullOrEmpty(FirstName) && string.IsNullOrWhiteSpace(FirstName?.Trim())) // !string.IsNullOrEmpty(FirstName) - Added to skip duplicate data annotation error message
            {
                AppendLine("First Name is required");
            }

            if (!string.IsNullOrEmpty(LastName) && string.IsNullOrWhiteSpace(LastName?.Trim()))
            {
                AppendLine("Last Name is required");
            }

            if (!string.IsNullOrEmpty(DisplayName) && string.IsNullOrWhiteSpace(DisplayName?.Trim()))
            {
                AppendLine("DisplayName is required");
            }

            if (!string.IsNullOrEmpty(Email) && string.IsNullOrWhiteSpace(Email?.Trim()))
            {
                AppendLine("Email is required");
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
