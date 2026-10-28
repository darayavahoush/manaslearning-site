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
using manaslearning.UI.Models;

namespace manaslearning.UI.Pages.User
{
    [Authorize]
    public class AssessmentInfoModel : PageModel
    {
        private readonly ILogger<AssessmentInfoModel> _logger;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IStudentRepository _studentRepository;

        private long userId;
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }
        public AssessmentInfo AssessmentInfo { get; set; }
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<Language> AssessmentLanguages { get; set; }

        public AssessmentInfoModel(
            ILogger<AssessmentInfoModel> logger,
            IAssessmentRepository assessmentRepository,
            IStudentRepository studentRepository
            )
        {
            _logger = logger;
            _assessmentRepository = assessmentRepository;
            _studentRepository = studentRepository;
        }

        public async Task OnGetAsync(string name, Guid id)
        {
            try
            {
                // TODO: validate 'name'
                // TODO: validate 'id'

                userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();
                //if (userId == 0)
                //    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                AssessmentInfo = await _assessmentRepository.GetAssessmentInfoByGuidIdAsync(organizationId, id);
                if (AssessmentInfo == null || !AssessmentInfo.HasLicense)
                {
                    throw new Exception("BadRequest");
                    // TODO: redirect to exception page
                }

                Students = await _studentRepository.GetStudentsAsync(userId);
                if (Students == null || !Students.Any())
                {
                    // TODO: redirect to exception page
                }

                // TODO: sort by language name
                // TODO: select english as default
                AssessmentLanguages = await _assessmentRepository.GetAssessmentLanguagesAsync(id);
                if (AssessmentLanguages == null || !AssessmentLanguages.Any())
                {
                    // TODO: redirect to exception page
                }

                SetBreadcrumb(AssessmentInfo);
            }
            catch (Exception ex)
            {
                // TODO: redirect to exception page
                _logger.LogError(ex, $"Error={ex.Message}, Source=AssessmentInfoModel.OnGetAsync(), UserId={userId}");
            }
        }

        private void SetBreadcrumb(AssessmentInfo assessmentInfo)
        {
            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = new BreadcrumbItem[] {
                    new BreadcrumbItem{ Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                    new BreadcrumbItem{ Title = "Assessments", Href = UrlConstants.USER_ASSESSMENTS },
                    new BreadcrumbItem { Title = assessmentInfo.Name }
                },
                IsResponsive = true
            };
        }
    }
}