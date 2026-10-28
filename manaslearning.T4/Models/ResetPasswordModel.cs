using manaslearning.T4.Templates.Models;

namespace manaslearning.T4.Templates.Models
{
    public class ResetPasswordModel
    {
        public Company Company { get; set; }
        public string Name { get; set; }
        public string ResetPasswordUrl { get; set; }
    }
}

