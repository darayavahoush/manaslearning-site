using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IStudentAssessmentRepository
    {
        Task<StudentAssessment> GetStudentAssessmentByIdAsync(long studentAssessmentId);

        Task<int> CreateStudentAssessmentAsync(Guid id, int studentId, int languageId, long userId);

        Task<Tuple<int, StudentAssessmentQuestion, IEnumerable<StudentAssessmentQuestionOption>>> GetStudentAssessmentNextQuestionAsync(int assessmentId, int studentId, int languageId, long userId);

        Task<int> UpdateStudentAssessmentQuestionAsync(long studentAssessmentId, int questionId, Guid questionGuidId, Guid questionOptionGuidId, long userId);

        Task<StudentAssessmentResult> GetAssessmentResultAsync(Guid id, int languageId, long userId);

        Task<int> GenerateAndSaveAssessmentResultAsync(Guid id, long userId);

        Task<StudentAssessment> GetExistingStudentAssessmentAsync(Guid id, int studentId, long userId);

        Task<int> DeleteStudentAssessmentByIdAsync(long studentAssessmentId);
    }
}