using manaslearning.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IFreeAssessmentRepository
    {
        // Free Assessments
        public Task<List<FreeAssessment>> GetFreeAssessments();
        public Task<FreeAssessment> GetFreeAssessmentById(int assessmentId);

        // Free Assessment Questions
        public Task<List<FreeAssessmentQuestion>> GetFreeAssessmentQuestions(int assessmentId);
    }
}
