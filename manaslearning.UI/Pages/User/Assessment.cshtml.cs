using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.Shared;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User
{
    [Authorize]
    public class AssessmentModel : PageModel
    {
        private readonly ILogger<AssessmentModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IStorageHelper _storageHelper;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IStudentAssessmentRepository _studentAssessmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ILicenseRepository _licenseRepository;        

        private long userId;
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }
        public int LanguageId { get; set; }
        public Assessment Assessment { get; set; }
        public IEnumerable<Language> AssessmentLanguages { get; set; }
        public Student Student { get; set; }
        public StudentAssessment StudentAssessment { get; set; }
        public Tuple<int, StudentAssessmentQuestion, IEnumerable<StudentAssessmentQuestionOption>> Question { get; set; }

        public float ProgressPercentage { get; set; } = 0;

        [TempData]
        public bool HasLicense { get; set; } = false;

        [TempData]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public PostModel PostModel { get; set; }

        public AssessmentModel(
            ILogger<AssessmentModel> logger,
            IConfiguration configuration,
            IStorageHelper storageHelper,
            IAssessmentRepository assessmentRepository,
            IStudentAssessmentRepository stduentAssessmentRepository,
            IStudentRepository studentRepository,
            ILicenseRepository licenseRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _storageHelper = storageHelper;
            _assessmentRepository = assessmentRepository;
            _studentAssessmentRepository = stduentAssessmentRepository;
            _studentRepository = studentRepository;
            _licenseRepository = licenseRepository;
        }

        /// <param name="name">Assessment Meta Name</param>
        /// <param name="id">Assessment GuidId</param>
        /// <param name="sid">StudentId</param>
        /// <param name="lid">LanguageId</param>
        public async Task<IActionResult> OnGetAsync(string name, Guid id, int sid, int lid)
        {
            try
            {
                long studentAssessmentId = 0;

                userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();
                if (userId == 0 || organizationId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                if (id.Equals(Guid.Empty))
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);

                if (sid <= 0)
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);

                if (lid <= 0)
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);

                LanguageId = lid;

                Student = await _studentRepository.GetStudentAsync(userId, sid);
                if (Student == null)
                {
                    _logger.LogError($"ErrorMessage=Student not found, Source=AssessmentModel.OnGetAsync(), sid={sid}, userId={userId}");
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);
                }

                Assessment = await _assessmentRepository.GetActiveAssessmentByGuidIdAsync(id);
                if (Assessment == null)
                {
                    _logger.LogError($"ErrorMessage=Assessment not found, Source=AssessmentModel.OnGetAsync(), AssessmentId={id}");
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);
                }

                // Get student existing assessment
                var existingStudentAssessment = await _studentAssessmentRepository.GetExistingStudentAssessmentAsync(id, sid, userId);
                if (existingStudentAssessment == null)
                {                    
                    // Check license
                    var license = await _licenseRepository.GetOrganizationLicenseByAssessmentIdAsync(organizationId, Assessment.AssessmentId);
                    if (license == null || license.AvailableLicenses <= 0)
                    {
                        this.ErrorMessage = "You do not have a valid license to take this assessment. Purchase licenses before you proceed.";
                        _logger.LogError($"ErrorMessage=The user does not have a license to proceed, Source=AssessmentModel.OnGetAsync(), AssessmentId={id}, AssessmentName={Assessment.Name}, userId={userId}");
                        return Page();
                    }

                    // Create student assessment
                    var newStudentAssessmentId = await _studentAssessmentRepository.CreateStudentAssessmentAsync(id, sid, lid, userId);
                    if (newStudentAssessmentId == -1)
                    {
                        // Note: No need log anything here.
                        return LocalRedirect(UrlConstants.USER_ERROR);
                    }
                    else
                        studentAssessmentId = newStudentAssessmentId;

                    // Reduce available license by 1.
                    var updateResult = await _licenseRepository.ReduceAvailableLicenseByOne(organizationId, license.AssessmentId);
                    if (updateResult == -1) // -1: ERROR  
                    {
                        // Note: No need log anything here.
                        var studentAssessmentDeleteResult = await _studentAssessmentRepository.DeleteStudentAssessmentByIdAsync(newStudentAssessmentId);
                        return LocalRedirect(UrlConstants.USER_ERROR);
                    }
                    else if (updateResult == -2) // -2: License record does not exist.
                    {
                        ErrorMessage = "You do not have a valid license to take this assessment. Purchase licenses before you proceed.";
                        _logger.LogError($"Error: {ErrorMessage} User:{userId}, AssessmentId:{license.AssessmentId}");
                        var studentAssessmentDeleteResult = await _studentAssessmentRepository.DeleteStudentAssessmentByIdAsync(newStudentAssessmentId);
                        return Page();
                    }
                    else if (updateResult == -3) // -3: The available licenses count is zero.
                    {
                        ErrorMessage = "All the licenses for this assessment were consumed. Purchase new licenses before starting the assessment.";
                        _logger.LogError($"Error: {ErrorMessage} User:{userId}, AssessmentId:{license.AssessmentId}");
                        var studentAssessmentDeleteResult = await _studentAssessmentRepository.DeleteStudentAssessmentByIdAsync(newStudentAssessmentId);
                        return Page();
                    }
                }
                else
                    studentAssessmentId = existingStudentAssessment.StudentAssessmentId;

                HasLicense = true;

                SetBreadcrumb(HasLicense);

                StudentAssessment = await _studentAssessmentRepository.GetStudentAssessmentByIdAsync(studentAssessmentId);
                if (StudentAssessment == null)
                {
                    _logger.LogError($"Source=AssessmentModel.OnGetAsync() Student assessment not found with StudentAssessmentId: {studentAssessmentId}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                if (StudentAssessment.StatusId == (int)AssessmentStatus.Completed)
                {
                    return new RedirectResult(url: $"/u/assessments/{this.Assessment.MetaUrl}/result/{this.Assessment.GuidId}/{this.StudentAssessment.StudentId}/{lid}");
                }

                // Get next assessment question
                Question = await _studentAssessmentRepository.GetStudentAssessmentNextQuestionAsync(Assessment.AssessmentId, sid, lid, userId);
                if (Question == null)
                {
                    _logger.LogError($"Source=AssessmentModel.OnGetAsync() Question not found");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }
                else if (Question.Item1 == -1) // Error occured
                {
                    // error occured, show message to user
                    // Hide remaining page
                    _logger.LogError($"Source=AssessmentModel.OnGetAsync() error occurred while reading question form repository");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }
                else if (Question.Item1 == 1) // Invalid Assessment Id.
                {
                    _logger.LogError($"Source=AssessmentModel.OnGetAsync() invalid assessmentId found");
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);
                }
                else if (Question.Item1 == 2) // All questions answered.
                {
                    // Redirect to result page.
                    return new RedirectResult(url: $"/u/assessments/{this.Assessment.MetaUrl}/result/{this.Assessment.GuidId}/{this.StudentAssessment.StudentId}/{lid}");
                }

                // Question.Item1 == 0) // Retrieve next question

                if (StudentAssessment != null
                    && Question?.Item2 != null
                    && Question.Item2.Sno - 1 > 0)
                {
                    ProgressPercentage = ((Question.Item2.Sno - 1) / (float)StudentAssessment.NumberOfQuestions) * 100;
                }

                AssessmentLanguages = await _assessmentRepository.GetAssessmentLanguagesAsync(id);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=AssessmentModel.OnGetAsync(), Id={id}, StudentId = {sid}, LanguageId={lid}, UserId={userId}");
                return new RedirectResult(url: $"/error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                userId = HttpContext.GetUserId();
                if (userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                if (!ModelState.IsValid || ModelState.Count != 6)
                {
                    _logger.LogError($"ErrorMessage=Invalid Model, Source=AssessmentModel.OnPostAsync(), " +
                        $"studentAssessmentId={PostModel.Param2}, " +
                        $"questionId={PostModel.Param3} " +
                        $"questionGuidId={PostModel.Param4}, " +
                        $"questionOptionGuidId={PostModel.Param6}, " +
                        $"userId={userId}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                var inputParamsString = $"studentAssessmentId={PostModel.Param2}, " +
                        $"questionId={PostModel.Param3} " +
                        $"questionGuidId={PostModel.Param4}, " +
                        $"questionOptionGuidId={PostModel.Param6}, " +
                        $"userId={userId}";

                Assessment = await _assessmentRepository.GetActiveAssessmentByGuidIdAsync(this.PostModel.Param1);
                if (Assessment == null)
                {
                    _logger.LogError($"ErrorMessage=Assessment can not be null, Source=AssessmentModel.OnPostAsync(), {inputParamsString}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                StudentAssessment = await _studentAssessmentRepository.GetStudentAssessmentByIdAsync(this.PostModel.Param2);
                if (StudentAssessment == null)
                {
                    _logger.LogError($"ErrorMessage=StudentAssessment can not be null, Source=AssessmentModel.OnPostAsync(), {inputParamsString}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                // Check if assessment is already completed
                if (StudentAssessment.StatusId == (int)AssessmentStatus.Completed)
                {
                    return new RedirectResult(url: $"/u/assessments/{this.Assessment.MetaUrl}/result/{this.Assessment.GuidId}/{this.StudentAssessment.StudentId}/{PostModel.Param5}");
                }

                var response = await _studentAssessmentRepository.UpdateStudentAssessmentQuestionAsync(
                        studentAssessmentId: PostModel.Param2,
                        questionId: PostModel.Param3,
                        questionGuidId: PostModel.Param4,
                        questionOptionGuidId: PostModel.Param6,
                        userId: userId
                    );

                /*
                    -- Return status:
                    -----------------
                     -1: error occured - API
                    200: question updated successfully, assessment in-progress
                    201: question does not exists for this user, bad request
                    202: assessment completed
                    203: error occured - SQL -- Note: This return status code is not in use
                */

                // error occured - API
                if (response == -1)
                {
                    _logger.LogError($"ErrorMessage=Error occured while saving the question, Source=AssessmentModel.OnPostAsync(), {inputParamsString}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }
                // question updated successfully, assessment in-progress
                else if (response == 200)
                {
                    // do nothing
                }
                // question does not exists for this user, bad request
                else if (response == 201)
                {
                    _logger.LogError($"ErrorMessage=Bad Reuest: Question does not exist for this user, Source=AssessmentModel.OnPostAsync(), {inputParamsString}");
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);
                }
                // assessment completed
                else if (response == 202)
                {
                    // Trigger email: Send excel report to psychologists group
                    var message = GetBase64StringMessage(
                        userId: userId,
                        studentId: StudentAssessment.StudentId,
                        assessmentId: Assessment.AssessmentId,
                        studentAssessmentId: StudentAssessment.StudentAssessmentId,
                        studentAssessmentOid: StudentAssessment.GuidId);

                    var storageAccountName = _configuration.GetValue<string>("StorageAccountName");
                    if (string.IsNullOrWhiteSpace(storageAccountName))
                    {
                        _logger.LogError($"ErrorMessage='StorageAccountName' secret does not exists in keyvault, Source=AssessmentModel.OnPostAsync(), {inputParamsString}");
                        return LocalRedirect(UrlConstants.USER_ERROR);
                    }

                    var assessmentExcelReportInputQueueName = _configuration.GetValue<string>("Queues:InputQueue:AssessmentReport");
                    if (string.IsNullOrWhiteSpace(assessmentExcelReportInputQueueName))
                    {
                        _logger.LogError($"ErrorMessage='AssessmentExcelReportInputQueueName' secret does not exists in keyvault, Source=AssessmentModel.OnPostAsync(), {inputParamsString}");
                        return LocalRedirect(UrlConstants.USER_ERROR);
                    }

                    var isSuccess = await _storageHelper.InsertMessageIntoQueueAsync(
                        storageAccountName: storageAccountName,
                        queueName: assessmentExcelReportInputQueueName,
                        message: message,
                        logger: _logger);

                    if (!isSuccess)
                    {                        
                       return LocalRedirect(UrlConstants.USER_ERROR);
                    }
                }

                return new RedirectResult(url: $"/u/assessments/{this.Assessment.MetaUrl}/{this.Assessment.GuidId}/{this.StudentAssessment.StudentId}/{PostModel.Param5}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"ErrorMessage={ex.Message}, Source=AssessmentModel.OnPostAsync(), " +
                        $"studentAssessmentId={PostModel.Param2}, " +
                        $"questionId={PostModel.Param3} " +
                        $"questionGuidId={PostModel.Param4}, " +
                        $"questionOptionGuidId={PostModel.Param6}, " +
                        $"userId={userId}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private void SetBreadcrumb(bool hasLicense = false)
        {
            var breadCrumbItems = new List<BreadcrumbItem>() {
                new BreadcrumbItem { Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                new BreadcrumbItem{ Title = "Assessments", Href = UrlConstants.USER_ASSESSMENTS }
            };

            if (hasLicense)
            {
                breadCrumbItems.Add(new BreadcrumbItem { Title = $"{Assessment.Name}", Href = $"{UrlConstants.USER_ASSESSMENTS}/{Assessment.MetaUrl}/{Assessment.GuidId}/info" });
            }

            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = breadCrumbItems.ToArray(),
                IsResponsive = true
            };
        }

        /// <summary>
        /// Get Base64 string message
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="studentId"></param>
        /// <param name="assessmentId"></param>
        /// <param name="studentAssessmentId"></param>
        /// <param name="studentAssessmentOid"></param>
        /// <returns>string</returns>
        private string GetBase64StringMessage(
            long userId,
            int studentId,
            int assessmentId,
            long studentAssessmentId,
            Guid studentAssessmentOid)
        {
            var message = new
            {
                InstanceId = Guid.NewGuid(),
                UserId = userId,
                StudentId = studentId,
                AssessmentId = assessmentId,
                StudentAssessmentId = studentAssessmentId,
                StudentAssessmentOid = studentAssessmentOid,
            };

            var plainText = JsonConvert.SerializeObject(message);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }

    public class PostModel
    {
        /// <summary>
        /// Assessment.GuidId
        /// </summary>
        public Guid Param1 { get; set; }

        /// <summary>
        /// StudentAssessmentQuestion.StudentAssessmentId
        /// </summary>
        public long Param2 { get; set; }

        /// <summary>
        /// StudentAssessmentQuestion.QuestionId
        /// </summary>
        public int Param3 { get; set; }

        /// <summary>
        /// StudentAssessmentQuestion.GuidId
        /// </summary>
        public Guid Param4 { get; set; }

        /// <summary>
        /// LanguageId
        /// </summary>
        public int Param5 { get; set; }

        /// <summary>
        /// StudentAssessmentQuestionOption.GuidId
        /// </summary>
        public Guid Param6 { get; set; }
    }
}