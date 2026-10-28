using System;

namespace manaslearning.Models
{
    public class Assessment
    {
        public int AssessmentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public string MetaUrl { get; set; }
        public bool IsActive { get; set; }
        public Guid GuidId { get; set; }
    }
}
