using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Helpers;
using manaslearning.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class ArticleTagRepository : IArticleTagRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public ArticleTagRepository(IApplicationConnection applicationConnection, ILogger<ArticleTagRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }
        
        /// <summary>
        /// Get Article Tags
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<ArticleTag>> GetArticleTagsAsync()
        {
            string procedureName = "[dbo].[GetArticleTags]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    
                    return await connection.QueryAsync<ArticleTag>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleTagRepository.GetArticleTagsAsync()");
                return null;
            }
        }

        /// <summary>
        /// Insert Categories
        /// </summary>        
        /// <param name="articleTagNames"></param>        
        /// <returns>
        /// bool
        ///     0:     Error     
        /// </returns>
        public async Task<bool> SaveArticleTagsAsync(
            string[] articleTagNames
        )
        {
            string procedureName = "[dbo].[SaveArticleTags]";
            try
            {
                var dtTagNames = articleTagNames.ConvertToDataTable();
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { Tags = dtTagNames.AsTableValuedParameter("TVP_String") };
                    return await connection.QueryFirstOrDefaultAsync<bool>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleTagRepository.SaveArticleTags()");
                return false;
            }
        }

        /// <summary>
        /// Get Article Tags by ArticleTagNames
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<ArticleTag>> GetArticleTagsByTagNamesAsync(string[] articleTagNames)
        {
            string procedureName = "[dbo].[GetArticleTagsByTagNames]";
            try
            {
                var dtTagNames = articleTagNames.ConvertToDataTable();
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { Tags = dtTagNames.AsTableValuedParameter("TVP_String") };
                    return await connection.QueryAsync<ArticleTag>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleTagRepository.GetArticleTagsByTagNamesAsync()");
                return null;
            }
        }
    }
}
