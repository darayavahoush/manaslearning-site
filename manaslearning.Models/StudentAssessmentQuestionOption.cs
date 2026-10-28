using System;

namespace manaslearning.Models
{
    public class StudentAssessmentQuestionOption
    {
        public long StudentAssessmentId { get; set; }
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
        public int Sno { get; set; }
        public string Option { get; set; }
        public string OptionTranslation { get; set; }
        public int Score { get; set; }
        public bool IsSelected { get; set; }
        public Guid GuidId { get; set; }
    }
}
