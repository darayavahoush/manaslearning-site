using static manaslearning.Models.Enums;

namespace manaslearning.Admin.API.Models
{
    public class UserDto
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string OrganizationName { get; set; }
        public LoginTypes LoginType { get; set; }
        public bool EmailVerified { get; set; }
        public bool IsSystemCreated { get; set; }
        public string LoginProvider { get; set; }
        public bool IsActive { get; set; }
    }
}
