namespace manaslearning.Models
{
    public class AssessmentQuestion
    {
        public int QuestionId { get; set; }
        public int AssessmentId { get; set; }
        public int Sno { get; set; }
        public string Category { get; set; }
        public string Question { get; set; }
    }
}
