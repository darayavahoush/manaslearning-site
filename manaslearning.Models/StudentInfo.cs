using System;

namespace manaslearning.Models
{
    public class StudentInfo
    {
        public int StudentId { get; set; } // TODO: change this to Guid
        public string StudentName { get; set; }
        public string ParentName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Zip { get; set; }
        public string LanguagesCanSpeak { get; set; } // It will hold multiple languages
        public DateTime DateOfBirth { get; set; }
        public string StudentClass { get; set; }
        public string ParentEducation { get; set; }
        public string Firstlanguage { get; set; }
        public string SecondLanguage { get; set; }
        public string ThirdLanguage { get; set; }
        public string MotherTongue { get; set; }
        public string Gender { get; set; }
        public string SchoolType { get; set; }
        public string SyllabusType { get; set; }
    }
}
