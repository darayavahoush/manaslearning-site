using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace manaslearning.UI.Pages
{
    public class SelfTestModel : PageModel
    {
        private readonly IFreeAssessmentRepository _freeAssessmentRepository;

        public FreeAssessment freeAssessment;
        public List<FreeAssessmentQuestion> freeAssessmentQuestions;

        public SelfTestModel(IFreeAssessmentRepository freeAssessmentRepository)
        {
            _freeAssessmentRepository = freeAssessmentRepository;
        }
        public async void OnGet(int testId)
        {
            try
            {
                // TODO: testId validation
                freeAssessment = await _freeAssessmentRepository.GetFreeAssessmentById(testId);
                freeAssessmentQuestions = await _freeAssessmentRepository.GetFreeAssessmentQuestions(testId);
            }
            catch (Exception)
            {
                throw; // Log exception
            }
        }
    }
}