using System.ComponentModel.DataAnnotations;
using System.Text;

namespace manaslearning.Admin.API.Models
{
    public class OrganizationAddressPostModel
    {
        [Required(ErrorMessage = "OrganizationOid is required")]        
        public Guid OrganizationOid { get; set; }

        [Required(ErrorMessage = "AddressTypeId is required")]
        public int AddressTypeId { get; set; }

        [Required(ErrorMessage = "AddressLine1 is required")]
        [StringLength(200, ErrorMessage = "AddressLine1 - Max. 200 characters allowed.")]
        public string AddressLine1 { get; set; }

        [StringLength(200, ErrorMessage = "AddressLine2 - Max. 200 characters allowed.")]
        public string AddressLine2 { get; set; }

        [StringLength(200, ErrorMessage = "AddressLine3 - Max. 200 characters allowed.")]
        public string AddressLine3 { get; set; }

        [StringLength(100, ErrorMessage = "City - Max. 100 characters allowed.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(100, ErrorMessage = "State - Max. 100 characters allowed.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100, ErrorMessage = "Country - Max. 100 characters allowed.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "ZipCode is required")]
        [StringLength(10, ErrorMessage = "ZipCode - Max. 100 characters allowed.")]
        public string ZipCode { get; set; }

        [StringLength(50, ErrorMessage = "Phone1 - Max. 50 characters allowed.")]
        public string Phone1 { get; set; }

        [StringLength(50, ErrorMessage = "Phone2 - Max. 50 characters allowed.")]
        public string Phone2 { get; set; }
        public bool IsDefault { get; set; }

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

            if (!string.IsNullOrEmpty(AddressLine1) && string.IsNullOrWhiteSpace(AddressLine1?.Trim())) // !string.IsNullOrEmpty(AddressLine1) - Added to skip duplicate data annotation error message
            {
                AppendLine("AddressLine1 is required");
            }

            if (!string.IsNullOrEmpty(State) && string.IsNullOrWhiteSpace(State?.Trim()))
            {
                AppendLine("State is required");
            }

            if (!string.IsNullOrEmpty(Country) && string.IsNullOrWhiteSpace(Country?.Trim()))
            {
                AppendLine("Country is required");
            }

            if (!string.IsNullOrEmpty(ZipCode) && string.IsNullOrWhiteSpace(ZipCode?.Trim()))
            {
                AppendLine("ZipCode is required");
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
