using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.ViewModels
{
    public class StudentAssessmentReportDto
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
