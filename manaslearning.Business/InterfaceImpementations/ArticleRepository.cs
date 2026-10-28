using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public ArticleRepository(IApplicationConnection applicationConnection, ILogger<ArticleRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }
        
        /// <summary>
        /// Get Articles
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Article>> GetArticlesAsync()
        {
            string procedureName = "[dbo].[GetArticles]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    
                    return await connection.QueryAsync<Article>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleRepository.GetArticlesAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get all Articles
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Article>> GetAllArticlesAsync()
        {
            string procedureName = "[dbo].[GetAllArticles]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {

                    return await connection.QueryAsync<Article>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleRepository.GetAllArticlesAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get Article
        /// </summary>       
        /// <param name="ArticleId"></param>
        /// <returns>Article</returns>
        public async Task<Article> GetArticleAsync(int ArticleId)
        {
            string procedureName = "[dbo].[GetArticle]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {                        
                        ArticleId = ArticleId
                    };
                    return await connection.QueryFirstOrDefaultAsync<Article>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleRepository.GetArticleAsync(), ArticleId={ArticleId}");
                return null;
            }
        }

        /// <summary>
        /// Get Recent Article
        /// </summary>       
        /// <returns>Article</returns>
        public async Task<Article> GetRecentArticleAsync()
        {
            string procedureName = "[dbo].[GetRecentArticle]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {                    
                    return await connection.QueryFirstOrDefaultAsync<Article>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleRepository.GetRecentArticleAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get Article
        /// </summary>       
        /// <param name="ArticleId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<int>> GetArticleRelatedLinksAsync(int ArticleId)
        {
            string procedureName = "[dbo].[GetArticleRelatedLinks]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        ArticleId = ArticleId
                    };
                    return await connection.QueryFirstOrDefaultAsync<IEnumerable<int>>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleRepository.GetArticleRelatedLinksAsync(), ArticleId={ArticleId}");
                return null;
            }
        }

        public async Task<int> CreateArticleAsync(string title, string content, string excerpt, string author, string categories, string tags, string featuredImage, string metaTitle, string metaUrl, string metaKeywords, string metaKeyDescription, bool isPendingReview, int createdBy)
        {
            string procedureName = "[dbo].[CreateArticle]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        Title = title,
                        Content = content,
                        Excerpt = excerpt,
                        Author = author,
                        Categories = categories,
                        Tags = tags,
                        FeaturedImage = featuredImage,
                        MetaTitle = metaTitle,
                        MetaUrl = metaUrl,
                        MetaKeywords = metaKeywords,
                        MetaKeyDescription = metaKeyDescription,
                        IsPendingReview = isPendingReview,
                        CreatedBy = createdBy
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ArticleRepository.CreateArticleAsync, Title={title}");
                return -1;
            }
        }

        public async Task<int> UpdateArticleAsync(int articleId, string title, string content, string excerpt, string author, string categories, string tags, string featuredImage, string metaTitle, string metaUrl, string metaKeywords, string metaKeyDescription, bool isPendingReview, int modifiedBy)
        {
            string procedureName = "[dbo].[UpdateArticle]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        ArticleId = articleId,
                        Title = title,
                        Content = content,
                        Excerpt = excerpt,
                        Author = author,
                        Categories = categories,
                        Tags = tags,
                        FeaturedImage = featuredImage,
                        MetaTitle = metaTitle,
                        MetaUrl = metaUrl,
                        MetaKeywords = metaKeywords,
                        MetaKeyDescription = metaKeyDescription,
                        IsPendingReview = isPendingReview,
                        ModifiedBy = modifiedBy
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ArticleRepository.UpdateArticleAsync, ArticleId={articleId}");
                return -1;
            }
        }

        public async Task<bool> DeleteArticleAsync(int articleId)
        {
            string procedureName = "[dbo].[DeleteArticle]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        ArticleId = articleId
                    };
                    return await connection.ExecuteScalarAsync<bool>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ArticleRepository.DeleteArticleAsync, ArticleId={articleId}");
                return false;
            }
        }

        public async Task<bool> IncrementArticleViewsAsync(int articleId)
        {
            string procedureName = "[dbo].[IncrementArticleViews]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        ArticleId = articleId
                    };
                    return await connection.ExecuteScalarAsync<bool>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ArticleRepository.IncrementArticleViewsAsync(), ArticleId={articleId}");
                return false;
            }
        }
    }
}
