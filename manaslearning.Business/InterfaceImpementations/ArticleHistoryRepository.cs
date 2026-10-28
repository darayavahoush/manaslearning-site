using Dapper;
using manaslearning.Business.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class ArticleHistoryRepository : IArticleHistoryRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public ArticleHistoryRepository(IApplicationConnection applicationConnection, ILogger<ArticleRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        public async Task<int> CreateArticleHistoryAsync(int articleId, string title, string content, string excerpt, string author, string categories, string tags, string featuredImage, string metaTitle, string metaUrl, string metaKeywords, string metaKeyDescription, int createdBy)
        {
            string procedureName = "[dbo].[CreateArticleHistory]";
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
                        CreatedBy = createdBy
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ArticleHistoryRepository.CreateArticleHistoryAsync, ArticleId={articleId}, Title={title}");
                return -1;
            }
        }


    }
}
