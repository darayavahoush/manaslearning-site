using System;

namespace manaslearning.Models
{
    public class StudentAssessment
    {
        public long StudentAssessmentId { get; set; }
        public long UserId { get; set; }
        public int StudentId { get; set; }
        public int AssessmentId { get; set; }
        public string Code { get; set; }
        public int LanguageId { get; set; }
        public int StatusId { get; set; }
        public int NumberOfQuestions { get; set; }
        public Guid GuidId { get; set; }
    }
}
