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
    public class ArticleCategoryRepository : IArticleCategoryRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public ArticleCategoryRepository(IApplicationConnection applicationConnection, ILogger<ArticleCategoryRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }
        
        /// <summary>
        /// Get Article Categories
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<ArticleCategory>> GetArticleCategoriesAsync()
        {
            string procedureName = "[dbo].[GetArticleCategories]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    
                    return await connection.QueryAsync<ArticleCategory>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleCategoryRepository.GetArticleCategoriesAsync()");
                return null;
            }
        }

        /// <summary>
        /// Insert Categories
        /// </summary>        
        /// <param name="articleCategoryNames"></param>        
        /// <returns>
        /// bool
        ///     0:     Error     
        /// </returns>
        public async Task<bool> SaveArticleCategoriesAsync(
            string[] articleCategoryNames
        )
        {
            string procedureName = "[dbo].[SaveArticleCategories]";
            try
            {
                var dtCategoryNames = articleCategoryNames.ConvertToDataTable();
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { Categories = dtCategoryNames.AsTableValuedParameter("TVP_String") };
                    return await connection.QueryFirstOrDefaultAsync<bool>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleCategoryRepository.SaveArticleCategoriesAsync()");
                return false;
            }
        }

        /// <summary>
        /// Get Article Categories by Category Names
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<ArticleCategory>> GetArticleCategoriesByCategoryNamesAsync(string[] articleCategoryNames)
        {
            string procedureName = "[dbo].[GetArticleCategoriesByCategoryNames]";
            try
            {
                var dtCategoryNames = articleCategoryNames.ConvertToDataTable();
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { Categories = dtCategoryNames.AsTableValuedParameter("TVP_String") };
                    return await connection.QueryAsync<ArticleCategory>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ArticleCategoryRepository.GetArticleCategoriesAsync()");
                return null;
            }
        }
    }
}
