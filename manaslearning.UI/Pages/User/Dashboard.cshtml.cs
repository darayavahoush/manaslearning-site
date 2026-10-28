using manaslearning.Business.Interfaces;
using manaslearning.UI.Helpers;
using manaslearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly ILogger _logger;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IStudentRepository _studentRepository;

        private long userId;
        public List<GenderwiseStudentCount> GenderwiseStudentCounts { get; set; }
        public int TotalStudentsCount { get; set; } = 0;
        public IEnumerable<RecentAssessment> InProgressAssessments { get; set; }
        public IEnumerable<RecentAssessment> RecentlyCompletedAssessments { get; set; }

        public DashboardModel(
            ILogger<DashboardModel> logger,
            IAssessmentRepository assessmentRepository,
            IStudentRepository studentRepository)
        {
            _logger = logger;
            _assessmentRepository = assessmentRepository;
            _studentRepository = studentRepository;
        }

        public async Task OnGetAsync()
        {
            try
            {
                userId = HttpContext.GetUserId();
                //if (userId == 0)
                //    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                int organizationId = HttpContext.GetOrganizationId();

                GenderwiseStudentCounts = (List<GenderwiseStudentCount>)await _studentRepository.GetStudentGenderCountByOrganizationIdAsync(organizationId);

                if (GenderwiseStudentCounts.Any())
                {
                    TotalStudentsCount = GenderwiseStudentCounts.Sum(x => x.Count);
                    GenderwiseStudentCounts.ForEach(x =>
                    {                        
                        x.IconName = "boy.svg";

                        if(x.GenderName == "Female")
                            x.IconName = "girl.svg";
                        else if (x.GenderName == "Male")
                            x.IconName = "boy.svg";
                        else if (x.GenderName == "Other")
                            x.IconName = "other.svg";
                    });
                }

                // Assessments
                var result = await _assessmentRepository.GetRecentAssessmentsByOrganizationIdAsync(organizationId);
                if (result?.Item1 != null)
                    InProgressAssessments = result.Item1;

                if (result?.Item2 != null)
                    RecentlyCompletedAssessments = result.Item2;
            }
            catch (Exception ex)
            {
                // TODO: redirect to exception page
                _logger.LogError(ex, $"Error={ex.Message}, Source=DashboardModel.OnGetAsync(), UserId={userId}");
            }
        }
    }
}