using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class ArticleRelatedLinksRepository : IArticleRelatedLinksRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public ArticleRelatedLinksRepository(IApplicationConnection applicationConnection, ILogger<ArticleRelatedLinksRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }


        /// <summary>
        /// Insert Or Delete Related Links for specific Article
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="relatedLinks"></param>  
        /// <returns>
        /// bool
        ///     0:     Error     
        /// </returns>
        public async Task<bool> SaveArticleRelatedLinksAsync(
            int articleId,
            int[] relatedLinks
        )
        {
            string procedureName = "[dbo].[SaveArticleRelatedLinks]";
            try
            {
                var dtRelatedLinks = relatedLinks.ConvertToDataTable();
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { ArticleId = articleId, RelatedLinks = dtRelatedLinks.AsTableValuedParameter("TVP_Int") };
                    return await connection.QueryFirstOrDefaultAsync<bool>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleRelatedLinksRepository.SaveArticleRelatedLinksAsync()");
                return false;
            }
        }
    }
}
