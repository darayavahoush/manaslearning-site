using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User
{
    [Authorize]
    public class AssessmentsModel : PageModel
    {
        private readonly ILogger _logger;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IItemRepository _itemRepository;        

        private long _userId;
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }
        public IList<AssessmentModel> Assessments { get; set; }

        public AssessmentsModel(
            ILogger<AssessmentsModel> logger,
            IAssessmentRepository assessmentRepository,
            IItemRepository itemRepository)
        {
            _logger = logger;
            _assessmentRepository = assessmentRepository;
            _itemRepository = itemRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();
                if (_userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                var assessments = await _assessmentRepository.GetAssessmentsInfoAsync(organizationId);
                if (assessments == null || !assessments.Any())
                    return LocalRedirect(UrlConstants.USER_ERROR);
                    
                await SetAssessments(assessments);

                SetBreadcrumb();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=AssessmentModel.OnGetAsync(), UserId={_userId}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private async Task SetAssessments(IEnumerable<AssessmentInfo> assessmentInfoCollection)
        {
            var items = await _itemRepository.GetItemsInfoAsync();
            if (items == null || items.Count() == 0)
            {
                _logger.LogCritical("Items not found.");
               throw new Exception("Items not found.");
            }

            Assessments = new List<AssessmentModel>();
            foreach (var assessment in assessmentInfoCollection)
            {
                var item = items.Where(i => i.AssessmentId == assessment.AssessmentId).FirstOrDefault();
                if (item == null)
                {
                    _logger.LogCritical($"Item not found for AssessmentId: {assessment.AssessmentId}");
                    throw new Exception($"Item not found for AssessmentId: {assessment.AssessmentId}");                    
                }

                var assessmentModel = new AssessmentModel()
                {
                    AssessmentId = assessment.AssessmentId,
                    Code = assessment.Code, // Note: do not use this property, I think value is not coming from procedure
                    Description = assessment.Description,
                    GuidId = assessment.GuidId,
                    HasLicense = assessment.HasLicense,
                    Instructions = assessment.Instructions,
                    Symptoms = AssessmentHelper.GetAssessmentSymptoms(assessment.Code),
                    ImageName = AssessmentHelper.GetAssessmentImageName(assessment.Code),
                    MetaUrl = assessment.MetaUrl,
                    Name = assessment.Name,
                    ItemOid = item.Oid
                };
                Assessments.Add(assessmentModel);
            }
        }

        private void SetBreadcrumb()
        {
            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = new BreadcrumbItem[] {
                    new BreadcrumbItem{ Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                    new BreadcrumbItem{ Title = "Assessments" }
                },
                IsResponsive = true
            };
        }

        public class AssessmentModel
        {
            public int AssessmentId { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Instructions { get; set; }
            public List<string> Symptoms { get; set; }
            public string ImageName { get; set; }
            public string MetaUrl { get; set; }            
            public Guid GuidId { get; set; }
            public bool HasLicense { get; set; }
            public Guid ItemOid { get; set; }
        }
    }
}