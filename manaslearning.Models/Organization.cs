using System;

namespace manaslearning.Models
{
    public class Organization
    {
        public Guid Oid { get; set; }        
        public int OrganizationId { get; set; }
        public int? ParentOrganizationId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int OrganizationTypeId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
