using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace manaslearning.UI.ViewModels
{
    public class OrganizationStudentClassPutModel
    {
        [Required(ErrorMessage = "StudentClassOid is required")]
        public Guid StudentClassOid { get; set; }        

        [Required(ErrorMessage = "StudentClassName is required")]
        [StringLength(50, ErrorMessage = "StudentClassName - Max. 50 characters allowed.")]
        public string StudentClassName { get; set; }
        
        [StringLength(256, ErrorMessage = "StudentClassDescription - Max. 256 characters allowed.")]
        public string StudentClassDescription { get; set; } 
        
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

            if (!string.IsNullOrEmpty(StudentClassName) && string.IsNullOrWhiteSpace(StudentClassName?.Trim())) // !string.IsNullOrEmpty(StudentClassName) - Added to skip duplicate data annotation error message
            {
                AppendLine("StudentClassName is required");
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
