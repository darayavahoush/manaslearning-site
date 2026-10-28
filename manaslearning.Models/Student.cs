using System;

namespace manaslearning.Models
{
    public class Student
    {
        public Guid Oid { get; set; }
        public int StudentId { get; set; }
        /// <summary>
        /// Teacher or Parent Id
        /// </summary>
        //public long UserId { get; set; }
        public string StudentName { get; set; } // 50 char
        public string ParentName { get; set; } // 50 char
        public string Email { get; set; } // 256 char
        public string  PhoneNumber { get; set; } // 15 char
        public string Zip { get; set; } // 10 char
        public string LanguagesCanSpeak { get; set; } // 500 char, It will hold multiple languages
        public DateTime DateOfBirth { get; set; }
        public int StudentClass { get; set; }
        public string StudentClassName { get; set; }
        public int ParentEducation { get; set; } // Optional
        public int Firstlanguage { get; set; }
        public int SecondLanguage { get; set; }
        public int ThirdLanguage { get; set; } // Optional
        public int MotherTongue { get; set; }
        public int Gender { get; set; }
        public int SchoolType { get; set; }
        public int SyllabusType { get; set; }
        public bool IsActive { get; set; }
    }
}
