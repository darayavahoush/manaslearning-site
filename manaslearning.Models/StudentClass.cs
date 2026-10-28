using System;

namespace manaslearning.Models
{
    public class StudentClass
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } 
        public string ClassDescription { get; set; }
        public bool IsActive { get; set; }
        public Guid Oid { get; set; }
    }
}
