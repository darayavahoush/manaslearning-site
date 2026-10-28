namespace manaslearning.Admin.API.Models
{
    public class OrganizationTeacherClassDto
    {
        public Guid TeacherClassOid { get; set; }
        public Guid StudentClassOid { get; set; }
        public long TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string StudentClassName { get; set; }
        public string StudentClassDescription { get; set; }
        public bool IsActive { get; set; }
    }
}
