using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IContactUsRepository
    {
        Task<int> SaveContactUs(string name, string email, string subject, string message);
    }
}
