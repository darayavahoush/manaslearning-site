using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace manaslearning.UI.Pages
{
    public class FreeAssessmentsModel : PageModel
    {
        private readonly IFreeAssessmentRepository _freeAssessmentRepository;
        private readonly ILogger<FreeAssessmentsModel> _logger;

        public List<FreeAssessment> freeAssessments;

        public FreeAssessmentsModel(IFreeAssessmentRepository freeAssessmentRepository, ILogger<FreeAssessmentsModel> logger)
        {
            _freeAssessmentRepository = freeAssessmentRepository;
            _logger = logger;
        }

        public async void OnGet()
        {
            try
            {
                freeAssessments = await _freeAssessmentRepository.GetFreeAssessments();
            }
            catch (Exception ex)
            {
                freeAssessments = new List<FreeAssessment>();
                _logger.LogError(ex, ex.Message, new { Source = "" });
            }

        }
    }
}