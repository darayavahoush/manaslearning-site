using manaslearning.Admin.API.Models;
using manaslearning.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace manaslearning.Admin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleCategoryController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IArticleCategoryRepository _articleCategoryRepository;

        public ArticleCategoryController(
            ILogger<ArticleCategoryController> logger,
            IArticleCategoryRepository articleCategoryRepository
            )
        {
            _logger = logger;
            _articleCategoryRepository = articleCategoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var articleCategories = await _articleCategoryRepository.GetArticleCategoriesAsync();
                if (articleCategories == null || !articleCategories.Any())
                    return NotFound();

                return Ok(articleCategories                                       
                    .Select(a => new ArticleCategoryDto
                    {
                       CategoryId = a.CategoryId,
                       CategoryName = a.CategoryName
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ArticleCategoryController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }        
    }
}
