using System;
using manaslearning.Models;

namespace manaslearning.UI.ViewModels
{
    public class OrganizationStudentClassDto
    {
        public Guid StudentClassOid { get; set; }
        public string StudentClassName { get; set; }
        public string StudentClassDescription { get; set; }
        public bool IsActive { get; set; }
    }
}
