using manaslearning.Business.Interfaces;
using manaslearning.UI.Helpers;
using manaslearning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages
{
    public class PaidAssessmentsModel : PageModel
    {
        private readonly ILogger _logger;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IItemRepository _itemRepository;

        public IList<PaidAssessmentModel> Assessments { get; set; }

        public PaidAssessmentsModel(
            ILogger<PaidAssessmentsModel> logger,
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
                long userId = HttpContext.GetUserId(); // userId might be 0 if the user didn't log in.
                int organizationId = HttpContext.GetOrganizationId();
                
                var assessments = await _assessmentRepository.GetAssessmentsInfoAsync(organizationId);

                await SetAssessments(assessments);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=PaidAssessmentsModel.OnGetAsync()");
                throw;
            }
        }

        private async Task SetAssessments(IEnumerable<AssessmentInfo> assessmentInfoCollection)
        {
            var items = await _itemRepository.GetItemsInfoAsync();
            if (items == null || items.Count() == 0)
            {
                _logger.LogCritical("ErrorMessage=Items not found, Source=PaidAssessmentsModel.OnGetAsync()");
                throw new Exception("Items not found.");
            }

            Assessments = new List<PaidAssessmentModel>();
            foreach (var assessment in assessmentInfoCollection)
            {
                var item = items.Where(i => i.AssessmentId == assessment.AssessmentId).FirstOrDefault();
                if (item == null)
                {
                    _logger.LogCritical($"ErrorMessage=Item not found for AssessmentId: {assessment.AssessmentId}, Source=PaidAssessmentsModel.OnGetAsync()");
                    throw new Exception($"Item not found for AssessmentId: {assessment.AssessmentId}");
                }

                var assessmentModel = new PaidAssessmentModel()
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
                    ItemOid = item.Oid,
                    Discount = item.Discount,
                    GSTAmount = item.GSTAmount,
                    ItemDescription = item.ItemDescription,
                    ItemPrice = item.ItemPrice,
                    
                };
                Assessments.Add(assessmentModel);
            }
        }

        public class PaidAssessmentModel
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
            public string ItemDescription { get; set; }
            public decimal ItemPrice { get; set; }
            public decimal GSTRate { get; set; }
            public decimal GSTAmount { get; set; }
            public decimal Discount { get; set; }
            public decimal MaximumDiscount { get; set; }
        }
    }
}
