using System;

namespace manaslearning.Models
{
    public class TeacherClass
    {
        public int OrganizationId { get; set; }
        public long TeacherId { get; set; }
        public int ClassId { get; set; }        
        public bool IsActive { get; set; }
        public Guid Oid { get; set; }
    }
}
