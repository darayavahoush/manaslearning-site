using manaslearning.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IArticleTagRepository
    {
        Task<IEnumerable<ArticleTag>> GetArticleTagsAsync();       
        Task<bool> SaveArticleTagsAsync(string[] articleTagNames);
        Task<IEnumerable<ArticleTag>> GetArticleTagsByTagNamesAsync(string[] articleTagNames);
    }
}