namespace manaslearning.Admin.API.Models
{
    public class OrganizationStudentDto
    {
        public Guid StudentOid { get; set; }
        public string StudentName { get; set; }
        public string ParentName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Zip { get; set; }
        public string LanguagesCanSpeak { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int StudentClass { get; set; }
        public string StudentClassName { get; set; }
        public int ParentEducation { get; set; }
        public int Firstlanguage { get; set; }
        public int SecondLanguage { get; set; }
        public int ThirdLanguage { get; set; }
        public int MotherTongue { get; set; }
        public int Gender { get; set; }
        public int SchoolType { get; set; }
        public int SyllabusType { get; set; }
        public bool IsActive { get; set; }
    }
}
