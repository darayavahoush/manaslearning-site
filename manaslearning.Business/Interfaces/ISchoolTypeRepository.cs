using manaslearning.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface ISchoolTypeRepository
    {
        Task<IEnumerable<SchoolType>> GetSchoolTypesAsync();
    }
}
