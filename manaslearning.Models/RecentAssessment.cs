using System;

namespace manaslearning.Models
{
    //TODO: Remove this model and use StudentAssessmentInfo
   [Obsolete("Remove this model and use StudentAssessmentInfo")]
    public class RecentAssessment
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string ParentName { get; set; }
        public string StudentClass { get; set; }
        public string AssessmentName { get; set; }
        public string AssessmentMetaUrl { get; set; }
        public Guid AssessmentGuidId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusId { get; set; }
        public Guid StudentAssessmentGuidId { get; set; }
    }
}
