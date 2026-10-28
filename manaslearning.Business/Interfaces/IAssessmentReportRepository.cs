using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IAssessmentReportRepository
    {
        Task<IEnumerable<StudentAssessmentInfo>> GetAsync(long userId, Paging pagingDTO);
        Task<IEnumerable<StudentAssessmentInfo>> GetByOrganizationIdAsync(int organizationId, Paging paging);

        /// <summary>
        /// Returns count of Student Assessments taken
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> GetAssessmentCountByUserAsyc(long userId);

        Task<int> GetAssessmentCountByOrganizationIdAsyc(int organizationId);
    }
}