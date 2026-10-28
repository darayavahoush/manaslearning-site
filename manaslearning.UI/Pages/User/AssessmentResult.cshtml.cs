using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User
{
    [Authorize]
    public class AssessmentResultModel : PageModel
    {
        private readonly ILogger<AssessmentInfoModel> _logger;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IStudentAssessmentRepository _studentAssessmentRepository;
        private readonly IStudentRepository _studentRepository;

        private long userId;
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }
        public int LanguageId { get; set; }
        public Assessment Assessment { get; set; }
        public Student Student { get; set; }
        public StudentAssessment StudentAssessment { get; set; }
        public StudentAssessmentResult StudentAssessmentResult { get; set; }

        public AssessmentResultModel(
            ILogger<AssessmentInfoModel> logger,
            IAssessmentRepository assessmentRepository,
            IStudentAssessmentRepository stduentAssessmentRepository,
            IStudentRepository studentRepository)
        {
            _logger = logger;
            _assessmentRepository = assessmentRepository;
            _studentAssessmentRepository = stduentAssessmentRepository;
            _studentRepository = studentRepository;
        }

        /// <param name="name">Assessment Meta Name</param>
        /// <param name="id">Assessment GuidId</param>
        /// <param name="sid">StudentId</param>
        /// <param name="lid">LanguageId</param>
        public async Task<IActionResult> OnGetAsync(string name, Guid id, int sid, int lid)
        {
            try
            {
                // TODO: validate 'name'
                // TODO: validate 'id'
                // TODO: validate 'sid'
                // TODO: validate 'lid'

                LanguageId = lid;

                userId = HttpContext.GetUserId();
                if (userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                Student = await _studentRepository.GetStudentAsync(userId, sid);
                if (Student == null)
                {
                    // TODO: redirect to exception page or 404 page
                }

                // TODO: Check if any assessment is in progress.

                Assessment = await _assessmentRepository.GetActiveAssessmentByGuidIdAsync(id);
                if (Assessment == null)
                {
                    // TODO: redirect to exception page or 404 page
                }

                // If No, create student assessment.
                var studentAssessmentId = await _studentAssessmentRepository.CreateStudentAssessmentAsync(id, sid, lid, userId);
                if (studentAssessmentId == -1)
                {
                    // TODO: error occured, show message to user
                }

                StudentAssessment = await _studentAssessmentRepository.GetStudentAssessmentByIdAsync(studentAssessmentId);
                if (StudentAssessment == null)
                {
                    // TODO: redirect to exception page or 404 page
                }

                if (StudentAssessment.StatusId == (int)AssessmentStatus.Completed)
                {
                    StudentAssessmentResult = await _studentAssessmentRepository.GetAssessmentResultAsync(StudentAssessment.GuidId, lid, userId);
                    if (StudentAssessmentResult == null) // result is not yet generated
                    {
                        int result = await _studentAssessmentRepository.GenerateAndSaveAssessmentResultAsync(StudentAssessment.GuidId, userId);
                        if (result == -1)
                        {
                            // internal server error
                            // TODO: redirect to exception page
                        }
                        else if (result == -2)
                        {
                            // assessment data null
                            // TODO: redirect to exception page
                        }
                        else if (result == -3)
                        {
                            // error while saving assessment result
                            // TODO: redirect to exception page
                        }
                        else
                        {
                            StudentAssessmentResult = await _studentAssessmentRepository.GetAssessmentResultAsync(StudentAssessment.GuidId, lid, userId);
                        }
                    }
                }
                else
                {
                    // TODO: redirect to assessment page
                }

                SetBreadcrumb();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=AssessmentModel.OnGetAsync(), Id={id}, StudentId = {sid}, LanguageId={lid}, UserId={userId}");
                return new RedirectResult(url: $"/error");
            }
        }

        private void SetBreadcrumb()
        {
            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = new BreadcrumbItem[] {
                    new BreadcrumbItem{ Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                    new BreadcrumbItem{ Title = "Assessment Reports", Href = UrlConstants.USER_ASSESSMENTS_REPORTS },
                    new BreadcrumbItem{ Title = Assessment.Name }
                },
                IsResponsive = true
            };
        }
    }
}
