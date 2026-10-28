using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface ILicenseHistoryRepository
    {
        Task<int> CreateLicenseHistoryAsync(long userId, int assessmentId, int itemId, int licenses, long createdBy);
    }
}
