using System;

namespace manaslearning.UI.ViewModels
{
    public class StudentDto
    {
        public int StudentId { get; set; } // TODO: change this to Guid
        public string StudentName { get; set; } // 50 char
        public string ParentName { get; set; } // 50 char
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } // 256 char
        public string PhoneNumber { get; set; } // 15 char
        public string StudentClass { get; set; }
    }
}
