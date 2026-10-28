namespace manaslearning.Admin.API.Models
{
    public class OrganizationStudentClassDto
    {
        public Guid StudentClassOid { get; set; }
        public string StudentClassName { get; set; }
        public string StudentClassDescription { get; set; }        
        public bool IsActive { get; set; }
    }
}
