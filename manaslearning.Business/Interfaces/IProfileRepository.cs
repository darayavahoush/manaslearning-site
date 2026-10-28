using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IProfileRepository
    {
        Task<long> CreateUserIdAsync(string id, string email);

        Task<long> CreateUserIdByEmailAsync(string email);

        Task<bool> CreateUserProfileAsync(
            long   id, 
            string firstName, 
            string lastName, 
            string displayName, 
            string email);

        Task<bool> UpdateUserProfileAsync(
            long id,
            string firstName,
            string lastName,
            string displayName,
            string email,
            bool isActive);

        Task<bool> UpdateUserPhoneNumberAsync(long userId, string phone);

        Task<bool> UpdateUserLoginTypeAsync(long userId, int loginType);

        Task<Profile> GetUserProfileAsync(long userId);

        Task<UserInfo> GetUserProfileByEmailIdAsync(string emailId);

        Task<long> CreateUserAsync(string firstName, string lastName, string displayName, string email, bool emailConfirmed, string phoneNumber, int loginType, string passWord, string securityStamp, string concurrencyStamp, int? organizationId, bool isSystemCreated);

        Task<IEnumerable<UserInfo>> GetUsersInfoAsync();

        Task<bool> ResetProfilePasswordAsync(long userId, string password);

        Task<IEnumerable<UserInfo>> GetAllUsersInfoByOrganizationOidAsync(Guid organizationOid);

        Task<IEnumerable<UserInfo>> GetUsersInfoByOrganizationOidAsync(Guid organizationOid);

        Task<UserInfo> GetUserInfoAsync(Guid organizationOid, long userId);

        Task<bool> UpdateUserClaimsAsync(long userId, string displayName);

        Task<int> DeleteUserProfileAsync(long userId);

        Task<IEnumerable<UserInfo>> GetAllUsersInfoByOrganizationIdAsync(int organizationId);

        Task<IEnumerable<UserInfo>> GetUsersInfoByOrganizationIdAsync(int organizationId);

        Task<UserInfo> GetUserInfoByOrganizationIdAsync(int organizationId, long userId);
    }
}
