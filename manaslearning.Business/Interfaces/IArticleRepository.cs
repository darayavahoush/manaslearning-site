using manaslearning.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IArticleRepository
    {
        Task<IEnumerable<Article>> GetArticlesAsync();
        Task<IEnumerable<Article>> GetAllArticlesAsync();
        Task<Article> GetArticleAsync(int ArticleId);
        Task<Article> GetRecentArticleAsync();
        Task<IEnumerable<int>> GetArticleRelatedLinksAsync(int articleId);

        Task<int> CreateArticleAsync(
            string title,
            string content,
            string excerpt,
            string author,
            string categories,
            string tags,
            string featuredImage,
            string metaTitle,
            string metaUrl,
            string metaKeywords,
            string metaKeyDescription,
            bool isPendingReview,
            int createdBy);

        Task<int> UpdateArticleAsync(
            int articleId, string title,
            string content, string excerpt,
            string author, string categories,
            string tags,
            string featuredImage,
            string metaTitle,
            string metaUrl,
            string metaKeywords,
            string metaKeyDescription,
            bool isPendingReview,
            int modifiedBy);

        Task<bool> DeleteArticleAsync(int articleId);

        Task<bool> IncrementArticleViewsAsync(int articleId);
    }
}