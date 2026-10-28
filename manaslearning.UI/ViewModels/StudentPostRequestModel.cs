namespace manaslearning.UI.ViewModels
{
    public class StudentPostRequestModel
    {
        public int RecordId { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Zip { get; set; }
        public string StudentCanSpeak { get; set; }
        public string Dob { get; set; }
        public int StudentClass { get; set; }
        public int ParentEducation { get; set; }
        public int FirstLanguage { get; set; }
        public int SecondLanguage { get; set; }
        public int ThirdLanguage { get; set; }
        public int MotherTongue { get; set; }
        public int Gender { get; set; }
        public int SchoolType { get; set; }
        public int SyllabusType { get; set; }
    }
}
