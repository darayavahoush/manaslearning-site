using manaslearning.Business.Interfaces;
using manaslearning.UI.Helpers;
using manaslearning.Models;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using manaslearning.UI.Models;

namespace manaslearning.UI.Controllers.User
{
    [Route("api/u/[controller]")]
    [ApiController]
    [Authorize]
    public class AssessmentReportController : ControllerBase
    {        
        private readonly IAssessmentReportRepository _assessmentReportRepository;
        private readonly ILogger _logger;

        public AssessmentReportController(
            IAssessmentReportRepository assessmentReportRepository,
            ILogger<AssessmentReportController> logger)
        {         
            this._assessmentReportRepository = assessmentReportRepository;
            this._logger = logger;
        }

        // GET api/studentassessmentreport
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] StudentAssessmentReportRequestModel model)
        {
            long _userId = 0;
            try
            {
                if (!model.IsValid())
                    return this.BadRequest();

                _userId = HttpContext.GetUserId();                
                var studentAssessments = await _assessmentReportRepository.GetAsync(_userId, model.ToPagingModel());                
                return Ok(studentAssessments.Select(s => new StudentAssessmentReportDto
                {
                    StudentId = s.StudentId,
                    StudentName = s.StudentName,
                    ParentName = s.ParentName,
                    AssessmentGuidId = s.AssessmentGuidId,
                    AssessmentMetaUrl = s.AssessmentMetaUrl,
                    AssessmentName = s.AssessmentName,
                    CreatedDate = s.CreatedDate,
                    StatusId = s.StatusId,
                    StudentAssessmentGuidId = s.StudentAssessmentGuidId,
                    StudentClass = s.StudentClass            
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=AssessmentReportController.GetAsync();UserId={_userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
