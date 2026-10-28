namespace manaslearning.Models
{
    public class FreeAssessment
    {
        public int AssessmentId { get; set; }
        public string AssessmentTitle { get; set; }
        public string AssessmentDescription { get; set; }
        public int LowerLimit { get; set; }
        public int UpperLimit { get; set; }
        public string DefaultMessage { get; set; }
        public string LowerLimitMessage { get; set; }
        public string UpperLimitMessage { get; set; }
    }
}
