using System;

namespace manaslearning.Models
{
    public class TeacherClassInfo
    {
        public Guid OrganizationOid { get; set; }
        public long TeacherId { get; set; }
        public string TeacherName { get; set; }       
        public Guid ClassOid { get; set; }        
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public bool IsActive { get; set; }
        public Guid Oid { get; set; }
    }
}
