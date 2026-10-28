using manaslearning.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IQualificationRepository
    {
        Task<IEnumerable<Qualification>> GetQualificationsAsync();
    }
}
