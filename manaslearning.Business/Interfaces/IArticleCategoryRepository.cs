using manaslearning.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IArticleCategoryRepository
    {
        Task<IEnumerable<ArticleCategory>> GetArticleCategoriesAsync();
        Task<bool> SaveArticleCategoriesAsync(string[] articleCategoryNames);
        Task<IEnumerable<ArticleCategory>> GetArticleCategoriesByCategoryNamesAsync(string[] articleCategoryNames);
    }
}