using System;

namespace manaslearning.Models
{
    public class AssessmentInfo
    {
        public int AssessmentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public string MetaUrl { get; set; }
        public bool IsActive { get; set; }
        public Guid GuidId { get; set; }
        public bool HasLicense { get; set; }
    }
}
