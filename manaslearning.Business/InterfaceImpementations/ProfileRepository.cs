using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public ProfileRepository(IApplicationConnection applicationConnection, ILogger<ContactUsRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        /// <summary>
        /// Create User Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>long</returns>
        public async Task<long> CreateUserIdAsync(string id, string email)
        {
            string procedureName = "[dbo].[CreateUserId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<long>(procedureName, param: new { Id = id, Email = email }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.CreateUserId");
                return -1;
            }
        }

        /// <summary>
        /// Create UserId by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>long</returns>
        public async Task<long> CreateUserIdByEmailAsync(string email)
        {
            string procedureName = "[dbo].[CreateUserIdByEmail]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<long>(procedureName, param: new { Email = email.ToLower() }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.CreateUserIdByEmailAsync");
                return -1;
            }
        }

        /// <summary>
        /// Create User Profile.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="displayName"></param>
        /// <param name="email"></param>
        /// <returns>bool</returns>
        public async Task<bool> CreateUserProfileAsync(long userId, string firstName, string lastName, string displayName, string email)
        {
            string procedureName = "[dbo].[CreateUserProfile]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        FirstName = firstName,
                        LastName = lastName,
                        DisplayName = displayName,
                        Email = email
                    };
                    return await connection.ExecuteScalarAsync<bool>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.CreateUserProfileAsync, UserId={userId}");
                return false;
            }
        }

        /// <summary>
        /// Update User Profile except Email.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="displayName"></param>
        /// <param name="phoneNumber"></param>
        /// <returns>bool</returns>
        public async Task<bool> UpdateUserProfileAsync(long userId, string firstName, string lastName, string displayName, string phoneNumber, bool isActive)
        {
            string procedureName = "[dbo].[UpdateUserProfile]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        FirstName = firstName,
                        LastName = lastName,
                        DisplayName = displayName,
                        Phone = phoneNumber,
                        IsActive = isActive
                    };
                    return await connection.ExecuteScalarAsync<bool>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.UpdateUserProfile, UserId={userId}");
                return false;
            }
        }

        /// <summary>
        /// Update User Claims .
        /// </summary>
        /// <param name="userId"></param>        
        /// <param name="displayName"></param>       
        /// <returns>bool</returns>
        public async Task<bool> UpdateUserClaimsAsync(long userId, string displayName)
        {
            string procedureName = "[dbo].[UpdateUserClaims]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,                        
                        DisplayName = displayName                       
                    };
                    return await connection.ExecuteScalarAsync<bool>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.UpdateUserClaimsAsync, UserId={userId}");
                return false;
            }
        }

        /// <summary>
        /// Update User Login Type.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginType"></param>
        /// <returns>bool</returns>
        public async Task<bool> UpdateUserLoginTypeAsync(long userId, int loginType)
        {
            string procedureName = "[dbo].[UpdateUserLoginType]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        LoginType = loginType
                    };
                    var rowsAffected = await connection.ExecuteAsync(procedureName, param: param, commandType: CommandType.StoredProcedure);
                    return rowsAffected > 0;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.UpdateUserLoginType, UserId={userId}");
                return false;
            }
        }

        /// <summary>
        /// Update Uesr Phone Number
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="phone"></param>
        /// <returns>bool</returns>
        public async Task<bool> UpdateUserPhoneNumberAsync(long userId, string phone)
        {
            string procedureName = "[dbo].[UpdateUserPhoneNumber]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        Phone = phone
                    };
                    var rowsAffected = await connection.ExecuteAsync(procedureName, param: param, commandType: CommandType.StoredProcedure);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.UpdateUserPhoneNumber, UserId={userId}");
                return false;
            }
        }

        /// <summary>
        /// Get User Profile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Profile</returns>
        public async Task<Profile> GetUserProfileAsync(long userId)
        {
            string procedureName = "[dbo].[GetUserProfile]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<Profile>(procedureName, param: new { UserId = userId }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.GetUserProfileAsync, UserId={userId}");
                return null;
            }
        }

        /// <summary>
        /// Get User Profile
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns>Profile</returns>
        public async Task<UserInfo> GetUserProfileByEmailIdAsync(string emailId)
        {
            string procedureName = "[dbo].[GetUserProfileByEmailId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<UserInfo>(procedureName, param: new { EmailId = emailId }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.GetUserProfileByEmailIdAsync, EmailId={emailId}");
                return null;
            }
        }

        /// <summary>
        /// Create User Profile.
        /// </summary>        
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="displayName"></param>
        /// <param name="email"></param>
        /// <param name="loginType"></param>
        /// <param name="passWord"></param>
        /// <param name="securityStamp"></param>
        /// <param name="concurrencyStamp"></param>
        /// <param name="organizationId"></param>
        /// <returns>long</returns>
        public async Task<long> CreateUserAsync(string firstName, string lastName, string displayName, string email, bool emailConfirmed, string phoneNumber, int loginType, string passWord, string securityStamp, string concurrencyStamp, int? organizationId, bool isSystemCreated)
        {
            string procedureName = "[dbo].[CreateUser]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {                        
                        FirstName = firstName,
                        LastName = lastName,
                        DisplayName = displayName,
                        Email = email,
                        EmailConfirmed = emailConfirmed,
                        LoginType = loginType,
                        PassWordHash = passWord,
                        SecurityStamp = securityStamp,
                        ConcurrencyStamp = concurrencyStamp,
                        OrganizationId = organizationId,
                        IsSystemCreated = isSystemCreated,
                        Phone = phoneNumber
                    };
                    return await connection.ExecuteScalarAsync<long>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.CreateSystemUserAsync, Email={email}");
                return -1;
            }
        }

        /// <summary>
        /// Get users info
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<UserInfo>> GetUsersInfoAsync()
        {
            string procedureName = "[dbo].[GetUsersInfo]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())                
                {
                    return await connection.QueryAsync<UserInfo>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ProfileRepository.GetUsersInfoAsync()");
                return null;
            }
        }

        /// <summary>
        /// Reset Profile Password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns>bool</returns>
        public async Task<bool> ResetProfilePasswordAsync(long userId, string password)
        {
            string procedureName = "[dbo].[ResetProfilePassword]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<bool>(procedureName, param: new { UserId = userId, Password = password }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.ResetProfilePasswordAsync, UserId={userId}");
                return false;
            }
        }

        /// <summary>
        /// Get All users info by Organization Oid
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<UserInfo>> GetAllUsersInfoByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetAllUsersInfoByOrganizationOid]";
            try
            {
                var param = new
                {
                    OrganizationOid = organizationOid
                };
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<UserInfo>(procedureName, param : param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ProfileRepository.GetAllUsersInfoByOrganizationOidAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get users info by Organization Oid
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<UserInfo>> GetUsersInfoByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetUsersInfoByOrganizationOid]";
            try
            {
                var param = new
                {
                    OrganizationOid = organizationOid
                };
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<UserInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ProfileRepository.GetUsersInfoByOrganizationOidAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get user info by Organization Oid and UserId
        /// </summary>       
        /// <returns>UserInfo</returns>
        public async Task<UserInfo> GetUserInfoAsync(Guid organizationOid, long userId)
        {
            string procedureName = "[dbo].[GetUserInfo]";
            try
            {
                var param = new
                {
                    OrganizationOid = organizationOid,
                    UserId = userId
                };
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<UserInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ProfileRepository.GetUserInfoAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get user info by Organization Id and UserId
        /// </summary>       
        /// <returns>UserInfo</returns>
        public async Task<UserInfo> GetUserInfoByOrganizationIdAsync(int organizationId, long userId)
        {
            string procedureName = "[dbo].[GetUserInfoByOrganizationId]";
            try
            {
                var param = new
                {
                    OrganizationId = organizationId,
                    UserId = userId
                };
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<UserInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ProfileRepository.GetUserInfoByOrganizationIdAsync()");
                return null;
            }
        }

        /// <summary>
        /// Delete UserProfile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>int</returns>
        public async Task<int> DeleteUserProfileAsync(long userId)
        {
            string procedureName = "[dbo].[DeleteUserProfile]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = ProfileRepository.DeleteUserProfile, UserId={userId}");
                return -1;
            }
        }

        /// <summary>
        /// Get All users info by Organization Id
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<UserInfo>> GetAllUsersInfoByOrganizationIdAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetAllUsersInfoByOrganizationId]";
            try
            {
                var param = new
                {
                    OrganizationId = organizationId
                };
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<UserInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ProfileRepository.GetAllUsersInfoByOrganizationIdAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get users info by Organization Id
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<UserInfo>> GetUsersInfoByOrganizationIdAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetUsersInfoByOrganizationId]";
            try
            {
                var param = new
                {
                    OrganizationId = organizationId
                };
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<UserInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ProfileRepository.GetUsersInfoByOrganizationIdAsync()");
                return null;
            }
        }
    }
}
