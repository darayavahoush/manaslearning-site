namespace manaslearning.Models
{
    public class AssessmentQuestionOption
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        public int Sno { get; set; }
        public string Option { get; set; }
        public int Score { get; set; }
    }
}
