using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IArticleHistoryRepository
    {
        Task<int> CreateArticleHistoryAsync(
            int articleId,
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
            int createdBy);
    }
}