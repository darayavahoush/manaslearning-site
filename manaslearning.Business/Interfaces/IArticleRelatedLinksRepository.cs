using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IArticleRelatedLinksRepository
    {
        Task<bool> SaveArticleRelatedLinksAsync(int articleId, int[] relatedLinks);

    }
}