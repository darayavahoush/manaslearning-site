using manaslearning.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IEnquiryRepository
    {
        Task<int> CreateEnquiryAsync(
            string name,
            string email,
            string phone,
            string city,
            string state,
            string country,
            int childAge,
            string serviceRequest,
            string message);

        Task<Enquiry> GetEnquiryAsync(long enquiryId);

        Task<IEnumerable<Enquiry>> GetEnquiriesAsync();
    }
}
