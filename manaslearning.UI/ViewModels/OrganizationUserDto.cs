using System;
using manaslearning.Models;

namespace manaslearning.UI.ViewModels
{
    public class OrganizationUserDto
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string OrganizationName { get; set; }
        public Enums.LoginTypes LoginType { get; set; }
        public bool EmailVerified { get; set; }
        public bool IsSystemCreated { get; set; }
        public string LoginProvider { get; set; }
        public bool IsActive { get; set; }
        public string ClassIds { get; set; }
    }
}
