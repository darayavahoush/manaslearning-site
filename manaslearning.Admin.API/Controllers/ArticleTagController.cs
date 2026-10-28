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
    public class ArticleTagController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IArticleTagRepository _articleTagRepository;

        public ArticleTagController(
            ILogger<ArticleTagController> logger,
            IArticleTagRepository articleTagRepository
            )
        {
            _logger = logger;
            _articleTagRepository = articleTagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var articleTags = await _articleTagRepository.GetArticleTagsAsync();
                if (articleTags == null || !articleTags.Any())
                    return NotFound();

                return Ok(articleTags                                       
                    .Select(a => new ArticleTagDto
                    {
                       TagId = a.TagId,
                       TagName = a.TagName
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ArticleTagController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }        
    }
}
