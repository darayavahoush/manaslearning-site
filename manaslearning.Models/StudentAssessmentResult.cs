namespace manaslearning.Models
{
    public class StudentAssessmentResult
    {
        public long StudentAssessmentId { get; set; }
        public int AssessmentId { get; set; }
        public bool HasProblem { get; set; }
        public string ResultCode { get; set; }
        public string Message { get; set; }
        public string MessageTranslation { get; set; }
    }
}
