using manaslearning.Admin.UI.Models;

namespace manaslearning.Admin.UI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserInfo>> GetUsersAsync();

        //Task<ApiResponseModel> GetUsersAsync();

        Task<ApiResponseModel> CreateBulkUsersAsync(IEnumerable<SchoolUserAccount> schoolAccounts);

        Task<ApiResponseModel> ResetPasswordAsync(string email);
    }
}
