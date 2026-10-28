using static manaslearning.Models.Enums;

namespace manaslearning.Models
{
    public class Profile
    {
        //public long UserId { get; set; } // Note: do not return UserId to UI

        // TODO: 50 characters
        public string FirstName { get; set; }
        // TODO: 50 characters
        public string LastName { get; set; }
        // TODO: 100 characters
        public string DisplayName { get; set; }
        // TODO: 256 characters
        public string Email { get; set; }
        // TODO: 15 characters
        public string Phone { get; set; }
        public LoginTypes LoginType { get; set; } // Teacher (or) Parent
        // TODO: 450 characters
        public string Id { get; set; } // Default property

        public int OrganizationId { get; set; }
    }
}
