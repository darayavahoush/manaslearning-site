using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IAssessmentRepository
    {
        Task<IEnumerable<Assessment>> GetActiveAssessmentsAsync();

        Task<Assessment> GetActiveAssessmentByGuidIdAsync(Guid id);

        Task<IEnumerable<Language>> GetAssessmentLanguagesAsync(Guid id);

        Task<Tuple<IEnumerable<RecentAssessment>, IEnumerable<RecentAssessment>>> GetRecentAssessmentsAsync(long userId);

        Task<Tuple<IEnumerable<RecentAssessment>, IEnumerable<RecentAssessment>>> GetRecentAssessmentsByOrganizationIdAsync(int organizationId);

        Task<IEnumerable<AssessmentInfo>> GetAssessmentsInfoAsync(int organizaionId);

        Task<AssessmentInfo> GetAssessmentInfoByGuidIdAsync(int organizationId, Guid id);
    }
}