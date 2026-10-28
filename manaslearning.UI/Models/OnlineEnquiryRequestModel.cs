using manaslearning.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace manaslearning.UI.Models
{
    public class OnlineEnquiryRequestModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name - Max. 150 characters allowed.")]
        public string Name { get; set; }
              
        public string Email { get; set; }

       
        [StringLength(50, ErrorMessage = "Max. 50 characters allowed.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "City - Invalid characters")]
        [StringLength(100, ErrorMessage = "City - Max. 100 characters allowed.")]
        public string City { get; set; }
        
        [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "State - Invalid characters.")]
        [StringLength(100, ErrorMessage = "State - Max. 100 characters allowed.")]
        public string State { get; set; }
        
        [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "Country - Invalid characters.")]
        [StringLength(100, ErrorMessage = "Country - Max. 100 characters allowed.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "ChildAge is required")]
        public int ChildAge { get; set; }       

        public string Message { get; set; }

        public OnlineEnquiryServiceRequest ServiceRequest { get; set; }

        public string Token { get; set; }

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

            if (!string.IsNullOrEmpty(Name) && string.IsNullOrWhiteSpace(Name?.Trim())) // !string.IsNullOrEmpty(Name) - Added to skip duplicate data annotation error message
            {
                AppendLine("Name is required");
            }            

            
            if (!string.IsNullOrEmpty(City) && string.IsNullOrWhiteSpace(City?.Trim()))
            {
                AppendLine("City is required");
            }

            if (!string.IsNullOrEmpty(Phone) || !string.IsNullOrEmpty(Email))
            {
                AppendLine("Email/Phone is required");
            }

            if (!string.IsNullOrEmpty(Email) && Email.IsValidEmail())
            {
                AppendLine("Email is invalid");
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

    public class OnlineEnquiryServiceRequest
    {
        public string[] Types { get; set; }
    }
}
