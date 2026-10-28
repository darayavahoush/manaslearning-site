using System.ComponentModel.DataAnnotations;

namespace manaslearning.Auth.API.Models
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string EmailId { get; set; }

        [Required]
        [MaxLength(4000)]
        public string IdToken { get; set; }
    }
}
