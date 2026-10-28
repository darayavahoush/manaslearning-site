namespace manaslearning.Admin.API.Models
{
    public class OrganizationTeacherClassPutModel
    {
        public Guid TeacherClassOid { get; set; }
        public Guid OrganizationOid { get; set; }
        public Guid StudentClassOid { get; set; }
        public long TeacherId { get; set; }        
        public bool IsActive { get; set; }
    }
}
