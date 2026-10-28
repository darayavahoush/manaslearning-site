namespace manaslearning.Admin.API.Models
{
    public class OrganizationTeacherClassPostModel
    {
        public Guid OrganizationOid { get; set; }
        public Guid StudentClassOid { get; set; }
        public long TeacherId { get; set; }        
    }
}
