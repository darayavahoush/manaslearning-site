using manaslearning.Admin.API.Attributes;
using manaslearning.Admin.API.Helpers;
using manaslearning.Admin.API.Models;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;

namespace manaslearning.Admin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationUserController : ControllerBase
    {
        private readonly ILogger _logger;     
        private readonly IProfileRepository _profileRepository;
        private readonly IOrganizationRepository _organizationRepository;

        public OrganizationUserController(
            ILogger<OrganizationUserController> logger,
            IOrganizationRepository organizationRepository,
            IProfileRepository profileRepository
            )
        {
            _logger = logger;            
            _profileRepository = profileRepository;
            _organizationRepository = organizationRepository;
        }

        [HttpGet("GetAllByOrganizationOid")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationUser_ReadAll)]
        public async Task<IActionResult> GetAllByOrganizationOidAsync(Guid organizationOid)
        {
            try
            {
                var organizationUsers = await _profileRepository.GetAllUsersInfoByOrganizationOidAsync(organizationOid);
                if (organizationUsers == null || !organizationUsers.Any())
                    return NotFound();

                return Ok(organizationUsers
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .Select(u => new OrganizationUserDto
                    {
                        Email = u.Email,
                        EmailVerified = u.EmailVerified,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        IsActive = u.IsActive,
                        LoginType = (Enums.LoginTypes)u.LoginType,
                        OrganizationName = u.OrganizationName,
                        UserId = u.UserId,
                        IsSystemCreated = u.IsSystemCreated,
                        LoginProvider = u.LoginProvider
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationUserController.GetAllByOrganizationOidAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("GetByOrganizationOid")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationUser_Read)]
        public async Task<IActionResult> GetByOrganizationOidAsync(Guid organizationOid)
        {
            try
            {
                var organizationUsers = await _profileRepository.GetUsersInfoByOrganizationOidAsync(organizationOid);
                if (organizationUsers == null || !organizationUsers.Any())
                    return NotFound();

                return Ok(organizationUsers
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .Select(u => new OrganizationUserDto
                    {
                        Email = u.Email,
                        EmailVerified = u.EmailVerified,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        IsActive = u.IsActive,
                        LoginType = (Enums.LoginTypes)u.LoginType,
                        OrganizationName = u.OrganizationName,
                        UserId = u.UserId,
                        IsSystemCreated = u.IsSystemCreated,
                        LoginProvider = u.LoginProvider
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationUserController.GetByOrganizationOidAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/OrganizationUser?OrganizationOid={OrganizationOid}&UserId={UserId}       
        [HttpGet("{OrganizationOid:Guid}/{UserId:long}")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationUser_Read)]
        public async Task<IActionResult> GetAsync(Guid organizationOid, long userId)
        {
            try
            {
                var model = await _profileRepository.GetUserInfoAsync(organizationOid, userId);
                if (model == null)
                { throw new Exception($"Error occured while fetching the Organization."); }               

                return Ok(new OrganizationUserDto
                {
                    Email = model.Email,
                    EmailVerified = model.EmailVerified,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsActive = model.IsActive,
                    LoginType = (Enums.LoginTypes)model.LoginType,
                    OrganizationName = model.OrganizationName,
                    UserId = model.UserId,
                    IsSystemCreated = model.IsSystemCreated,
                    LoginProvider = model.LoginProvider
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationUserController.GetAsync(), OrganizationOid:{organizationOid}, UserId:{userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        // POST api/OrganizationUser        
        [HttpPost]
        [SuppressModelStateInvalidFilter]
        [AllowedPrivileges(PrivilegeTypes.OrganizationUser_Create)]
        public async Task<IActionResult> PostAsync([FromBody] OrganizationUserPostModel model)
        {            
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationUserController.PostAsync(), DisplayName: {model.DisplayName}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationByOidAsync(model.OrganizationOid);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationUserController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                PasswordHashHelper passwordHashHelper = new PasswordHashHelper();
                string password = passwordHashHelper.GenerateRandomPassword();

                long userId = await _profileRepository.CreateUserAsync
                                        (                                            
                                            model.FirstName.Trim(),
                                            model.LastName.Trim(),
                                            model.DisplayName.Trim(),
                                            model.Email.Trim(),
                                            model.EmailConfirmed,
                                            model.PhoneNumber?.Trim(),
                                            model.LoginType,
                                            passwordHashHelper.HashPassword(password),
                                            PasswordHashHelper.NewSecurityStamp(),
                                            PasswordHashHelper.GenerateConcurrencyStampAsync(),
                                            organization.OrganizationId,
                                            false
                                        );                                          

                if (userId < 0)
                { throw new Exception($"Error occured while inserting the Organization User. Email: {model.Email}, UserId: {userId}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationUserController.PostAsync(), DisplayName: {model.DisplayName}, Email: {model.Email}, OrganizationId: {model.OrganizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/OrganizationUser        
        [HttpPut]
        [AllowedPrivileges(PrivilegeTypes.OrganizationUser_Update)]
        public async Task<IActionResult> PutAsync([FromBody] OrganizationUserPutModel model)
        {
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationUserController.PutAsync(), UserId: {model.UserId}");
                    return this.BadRequest();
                }

                var profile = await _profileRepository.GetUserInfoAsync(model.OrganizationOid, model.UserId);
                if (profile == null) 
                {
                    _logger.LogError($"Profile is null. Source:OrganizationUserController.PutAsync(), UserId: {model.UserId}");
                    return this.BadRequest();
                }               

                var isOrganizationUserUpdated = await _profileRepository.UpdateUserProfileAsync(
                        model.UserId,
                        model.FirstName,
                        model.LastName,
                        model.DisplayName,
                        model.PhoneNumber,
                        model.IsActive
                    );

                if (!isOrganizationUserUpdated) 
                {
                    _logger.LogError($"Error occured while update profile. Source:OrganizationUserController.PutAsync(), UserId: {model.UserId}");
                    return this.BadRequest();
                }

                var isClaimsUpdated = await _profileRepository.UpdateUserClaimsAsync(model.UserId, model.DisplayName);
                if (!isOrganizationUserUpdated)
                {
                    _logger.LogError($"Error occured while update the User Claims. Source:OrganizationUserController.PutAsync(), UserId: {model.UserId}");
                    return this.BadRequest();
                }

                _logger.LogInformation("User profile updated.");

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationUserController.PutAsync(), UserId: {model.UserId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // DELETE api/OrganizationUser/oid        
        [HttpDelete]
        [AllowedPrivileges(PrivilegeTypes.OrganizationUser_Delete)]
        public async Task<IActionResult> DeleteAsync(Guid organizationOid, long userId)
        {
            try
            {
                var profile = await _profileRepository.GetUserInfoAsync(organizationOid, userId);
                if (profile == null)
                {
                    _logger.LogError($"Profile is null. Source:OrganizationUserController.DeleteAsync(), UserId: {userId}, OrganizationOid: {organizationOid}");
                    return this.BadRequest();
                }

                int result = await _profileRepository.DeleteUserProfileAsync(userId);
                if (result != 1)
                { throw new Exception($"Error occured while deleting the User, UserId : {userId}"); }
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationUserController.DeleteAsync(), UserId: {userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("[Action]")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationUser_ResetPassword)]
        public async Task<IActionResult> ResetPasswordAsync(Guid organizationOid, string emailId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId))
                    return new StatusCodeResult((int)HttpStatusCode.BadRequest);

                var user = await _profileRepository.GetUserProfileByEmailIdAsync(emailId);
                // !user.IsSystemCreated - Removed this condition to reset password for all active users.
                // !string.IsNullOrWhiteSpace(user.LoginProvider) - Removed this condition to generate the password even the user doesn't have the LoginProvider.
                if (user == null || user.OrganizationOid != organizationOid)  
                    return new StatusCodeResult((int)HttpStatusCode.BadRequest);

                PasswordHashHelper passwordHashHelper = new PasswordHashHelper();
                string password = passwordHashHelper.GenerateRandomPassword();

                if (!await _profileRepository.ResetProfilePasswordAsync(user.UserId, passwordHashHelper.HashPassword(password)))
                    return this.StatusCode((int)HttpStatusCode.InternalServerError);

                return Ok(password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=UserController.ResetPasswordAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
