namespace manaslearning.Admin.API.Models
{
    public class SchoolUserAccountResponseModel
    {
        public string SchoolName { get; set; }
        public string Email { get; set; }
        public string TeacherFirstName { get; set; }
        public string TeacherLastName { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}
