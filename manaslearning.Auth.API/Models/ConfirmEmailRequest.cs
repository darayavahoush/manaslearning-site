using System.ComponentModel.DataAnnotations;

namespace manaslearning.Auth.API.Models
{
    public class ConfirmEmailRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
