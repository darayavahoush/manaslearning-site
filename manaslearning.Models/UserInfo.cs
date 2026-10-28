using System;

namespace manaslearning.Models
{
    public class UserInfo
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public Guid OrganizationOid { get; set; }
        public string OrganizationName { get; set; }
        public int LoginType { get; set; }
        public bool EmailVerified { get; set; }
        public bool IsSystemCreated { get; set; }
        public string LoginProvider { get; set; }
        public bool IsActive { get; set; }        
    }
}
