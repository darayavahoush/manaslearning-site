using manaslearning.Admin.API.Attributes;
using manaslearning.Admin.API.Models;
using manaslearning.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace manaslearning.Admin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleTagRepository _articleTagRepository;
        private readonly IArticleCategoryRepository _articleCategoryRepository;
        private readonly IArticleRelatedLinksRepository _articleRelatedLinksRepository;
        private readonly IArticleHistoryRepository _articleHistoryRepository;

        public ArticleController(
            ILogger<ArticleController> logger,
            IArticleRepository articleRepository,
            IArticleTagRepository articleTagRepository,
            IArticleCategoryRepository articleCategoryRepository,
            IArticleRelatedLinksRepository articleRelatedLinksRepository,
            IArticleHistoryRepository articleHistoryRepository
            )
        {
            _logger = logger;
            _articleRepository = articleRepository;
            _articleTagRepository = articleTagRepository;
            _articleCategoryRepository = articleCategoryRepository;
            _articleRelatedLinksRepository = articleRelatedLinksRepository;
            _articleHistoryRepository = articleHistoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var articles = await _articleRepository.GetArticlesAsync();
                if (articles == null || !articles.Any())
                    return NotFound();

                return Ok(articles
                    .OrderByDescending(x => x.ArticleId)                    
                    .Select(a => new ArticleDto
                    {
                       ArticleId = a.ArticleId,
                       Author = a.Author,
                       Categories = a.Categories,
                       Excerpt = a.Excerpt,
                       FeaturedImage = a.FeaturedImage,                       
                       IsActive = a.IsActive,
                       IsPendingReview = a.IsPendingReview,
                       MetaKeyDescription = a.MetaKeyDescription,
                       MetaKeyWords = a.MetaKeyWords,
                       MetaTitle = a.MetaTitle,
                       MetaUrl = a.MetaUrl,                       
                       Tags = a.Tags,
                       Title = a.Title,
                       Views = a.Views
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ArticleController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/Article?ArticleId={articleId}
        [HttpGet("{articleId:int}")]
        public async Task<IActionResult> GetAsync(int articleId)
        {
            try
            {
                var model = await _articleRepository.GetArticleAsync(articleId);
                if (model == null)
                { throw new Exception($"Error occured while fetching the Article."); } 

                await _articleRepository.IncrementArticleViewsAsync(articleId);

                return Ok(new ArticleDto
                {
                    ArticleId = model.ArticleId,
                    Author = model.Author,
                    Categories = model.Categories,
                    Content = model.Content,
                    Excerpt = model.Excerpt,
                    FeaturedImage = model.FeaturedImage,
                    IsActive = model.IsActive,
                    IsPendingReview = model.IsPendingReview,
                    MetaKeyDescription = model.MetaKeyDescription,
                    MetaKeyWords = model.MetaKeyWords,
                    MetaTitle = model.MetaTitle,
                    MetaUrl = model.MetaUrl,                   
                    Tags = model.Tags,
                    Title = model.Title,
                    Views = model.Views
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ArticleController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> GetRecentArticleAsync()
        {
            try
            {
                var model = await _articleRepository.GetRecentArticleAsync();
                return Ok(new ArticleDto
                {
                    ArticleId = model.ArticleId,
                    Author = model.Author,
                    Categories = model.Categories,
                    Content = model.Content,
                    Excerpt = model.Excerpt,
                    FeaturedImage = model.FeaturedImage,
                    IsActive = model.IsActive,
                    IsPendingReview = model.IsPendingReview,
                    MetaKeyDescription = model.MetaKeyDescription,
                    MetaKeyWords = model.MetaKeyWords,
                    MetaTitle = model.MetaTitle,
                    MetaUrl = model.MetaUrl,
                    Tags = model.Tags,
                    Title = model.Title,
                    Views = model.Views
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ArticleController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/Article/RelatedLinks?ArticleId={articleId}
        [HttpGet("[Action]")]
        public async Task<IActionResult> GetRelatedLinksAsync(int articleId)
        {
            try
            {
                var result = await _articleRepository.GetArticleRelatedLinksAsync(articleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ArticleController.GetRelatedLinksAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/Article        
        [HttpPost]
        [SuppressModelStateInvalidFilter]
        public async Task<IActionResult> PostAsync([FromBody] ArticlePostModel model)
        {            
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    return this.BadRequest();
                }               

                // Insert new article categories
                if (model.Categories.Any())
                {
                    var categoriesSaveResult = await _articleCategoryRepository.SaveArticleCategoriesAsync(model.Categories);
                    if (!categoriesSaveResult)
                    { throw new Exception($"Error occured while inserting Categories"); }

                }

                // Insert new article tags
                if (model.Tags.Any())
                {
                    var tagsSavetResult = await _articleTagRepository.SaveArticleTagsAsync(model.Tags);
                    if (!tagsSavetResult)
                    { throw new Exception($"Error occured while inserting Tags"); }

                }

                var articleTags = await _articleTagRepository.GetArticleTagsByTagNamesAsync(model.Tags);
                var articleCategories = await _articleCategoryRepository.GetArticleCategoriesByCategoryNamesAsync(model.Categories);

                int articleId = await _articleRepository.CreateArticleAsync
                                        (
                                            model.Title,
                                            model.Content,
                                            model.Excerpt,
                                            model.Author,
                                            JsonConvert.SerializeObject(articleCategories),
                                            JsonConvert.SerializeObject(articleTags),
                                            model.FeaturedImage,
                                            model.MetaTitle,
                                            model.MetaUrl,
                                            model.MetaKeyWords,
                                            model.MetaKeyDescription,
                                            model.IsPendingReview,
                                            0 // UserId
                                        );

                if (articleId < 0)
                { throw new Exception($"Error occured while inserting the Article. ArticleId: {articleId}"); }

                if (model.RelatedLinks != null && model.RelatedLinks.Length > 0)
                {
                    bool relatedLinksResult = await _articleRelatedLinksRepository.SaveArticleRelatedLinksAsync(articleId, model.RelatedLinks);
                    if(!relatedLinksResult)
                    { throw new Exception($"Error occured while inserting the Article Related Links, ArticleId : {articleId}"); }
                }

                int articleHistoryId = await _articleHistoryRepository.CreateArticleHistoryAsync
                                        (
                                            articleId,
                                            model.Title,
                                            model.Content,
                                            model.Excerpt,
                                            model.Author,
                                            JsonConvert.SerializeObject(articleCategories),
                                            JsonConvert.SerializeObject(articleTags),
                                            model.FeaturedImage,
                                            model.MetaTitle,
                                            model.MetaUrl,
                                            model.MetaKeyWords,
                                            model.MetaKeyDescription,                                            
                                            0 // UserId
                                        );


                if (articleHistoryId < 0)
                { throw new Exception($"Error occured while inserting the Article History. ArticleId: {articleId}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:ArticleController.PostAsync(), Title: {model.Title}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/Article        
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] ArticlePutModel model)
        {
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    return this.BadRequest();
                }

                // Insert new article categories
                if (model.Categories.Any())
                {
                    var categoriesSaveResult = await _articleCategoryRepository.SaveArticleCategoriesAsync(model.Categories);
                    if (!categoriesSaveResult)
                    { throw new Exception($"Error occured while inserting Categories"); }

                }

                // Insert new article tags
                if (model.Tags.Any())
                {
                    var tagsSavetResult = await _articleTagRepository.SaveArticleTagsAsync(model.Tags);
                    if (!tagsSavetResult)
                    { throw new Exception($"Error occured while inserting Tags"); }

                }

                var articleTags = await _articleTagRepository.GetArticleTagsByTagNamesAsync(model.Tags);
                var articleCategories = await _articleCategoryRepository.GetArticleCategoriesByCategoryNamesAsync(model.Categories);

                int articleHistoryId = await _articleHistoryRepository.CreateArticleHistoryAsync
                                        (
                                            model.ArticleId,
                                            model.Title,
                                            model.Content,
                                            model.Excerpt,
                                            model.Author,
                                            JsonConvert.SerializeObject(articleCategories),
                                            JsonConvert.SerializeObject(articleTags),
                                            model.FeaturedImage,
                                            model.MetaTitle,
                                            model.MetaUrl,
                                            model.MetaKeyWords,
                                            model.MetaKeyDescription,
                                            0 // UserId
                                        );


                if (articleHistoryId < 0)
                { throw new Exception($"Error occured while inserting the Article History. ArticleId: {model.ArticleId}"); }



                int articleId = await _articleRepository.UpdateArticleAsync
                                        (
                                            model.ArticleId,
                                            model.Title,
                                            model.Content,
                                            model.Excerpt,
                                            model.Author,
                                            JsonConvert.SerializeObject(articleCategories),
                                            JsonConvert.SerializeObject(articleTags),
                                            model.FeaturedImage,
                                            model.MetaTitle,
                                            model.MetaUrl,
                                            model.MetaKeyWords,
                                            model.MetaKeyDescription,
                                            model.IsPendingReview,
                                            0 // UserId
                                        );

                if (articleId < 0)
                { throw new Exception($"Error occured while updating the Article. ArticleId: {articleId}"); }

                if (model.RelatedLinks != null && model.RelatedLinks.Length > 0)
                {
                    bool relatedLinksResult = await _articleRelatedLinksRepository.SaveArticleRelatedLinksAsync(articleId, model.RelatedLinks);
                    if (!relatedLinksResult)
                    { throw new Exception($"Error occured while inserting the Article Related Links, ArticleId : {articleId}"); }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:ArticleController.PutAsync(), Title: {model.Title}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // DELETE api/Article/ArticleId        
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int articleId)
        {
            try
            {
                bool relatedLinksResult = await _articleRepository.DeleteArticleAsync(articleId);
                if (!relatedLinksResult)
                { throw new Exception($"Error occured while inserting the Article Related Links, ArticleId : {articleId}"); }
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:ArticleController.DeleteAsync(), ArticleId: {articleId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
