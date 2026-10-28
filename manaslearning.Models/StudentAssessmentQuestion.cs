using System;

namespace manaslearning.Models
{
    public class StudentAssessmentQuestion
    {
        public long StudentAssessmentId { get; set; }
        public int QuestionId { get; set; }
        public int Sno { get; set; }
        public string Category { get; set; }
        public string Question { get; set; }
        public string QuestionTranslation { get; set; }
        public int NumberOfQuestions { get; set; }
        public Guid GuidId { get; set; }
    }
}
