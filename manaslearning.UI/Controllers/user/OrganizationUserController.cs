using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Attributes;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace manaslearning.UI.Controllers.user
{
    [Route("api/u/[controller]")]
    [ApiController]
    [Authorize]
    public class OrganizationUserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IProfileRepository _profileRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ITeacherClassRepository _teacherClassRepository;

        public OrganizationUserController(
            ILogger<OrganizationUserController> logger,
            IOrganizationRepository organizationRepository,
            IProfileRepository profileRepository,
            ITeacherClassRepository teacherClassRepository
            )
        {
            _logger = logger;
            _profileRepository = profileRepository;
            _organizationRepository = organizationRepository;
            _teacherClassRepository = teacherClassRepository;
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> AllAsync()
        {
            try
            {
                int organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationUserController.GetAll();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                var organizationUsers = await _profileRepository.GetAllUsersInfoByOrganizationIdAsync(organizationId);
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
                        LoginProvider = u.LoginProvider,
                        DisplayName = u.DisplayName
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationUserController.GetAllAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                int organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationUserController.GetAll();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                var organizationUsers = await _profileRepository.GetUsersInfoByOrganizationIdAsync(organizationId);
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
                        LoginProvider = u.LoginProvider,
                        DisplayName = u.DisplayName
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationUserController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/OrganizationUser?UserId={UserId}       
        [HttpGet("{userId:long}")]
        public async Task<IActionResult> GetAsync(long userId)
        {
            try
            {
                int organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationUserController.GetAll();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                var model = await _profileRepository.GetUserInfoByOrganizationIdAsync(organizationId, userId);
                if (model == null)
                { throw new Exception($"Error occured while fetching the Organization. OrganizationId:{organizationId}, UserId:{userId}"); }

                var teacherClasses = await _teacherClassRepository.GetTeacherClassesAsync(model.OrganizationOid, userId);


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
                    LoginProvider = model.LoginProvider,
                    DisplayName = model.DisplayName,
                    ClassIds = teacherClasses.Any() ? string.Join(",", teacherClasses.Select(tc => tc.ClassId)) : string.Empty
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationUserController.GetAsync(), UserId:{userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/OrganizationUser        
        [HttpPost]
        [SuppressModelStateInvalidFilter]
        public async Task<IActionResult> PostAsync([FromForm] OrganizationUserPostModel model)
        {
            int organizationId = 0;
            long organizationAdmin = 0;
            try
            {
                organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationUserController.PostAsync();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                organizationAdmin = HttpContext.GetUserId();
                if (organizationAdmin == 0)
                {
                    _logger.LogError($"Error=Incorrect UserId. Source=OrganizationUserController.PostAsync();OrganizationId={organizationId},UserId={organizationAdmin}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationUserController.PostAsync(), DisplayName: {model.DisplayName}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationAsync(organizationId);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationUserController.PostAsync(), OrganizationId: {organizationId}");
                    return this.BadRequest();
                }

                var userInfo = await _profileRepository.GetUserProfileByEmailIdAsync(model.Email);
                if (userInfo != null)
                { return this.Conflict("Email was already registered"); }

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
                                            (int)Enums.LoginTypes.OrganiztionUser,
                                            passwordHashHelper.HashPassword(password),
                                            PasswordHashHelper.NewSecurityStamp(),
                                            PasswordHashHelper.GenerateConcurrencyStampAsync(),
                                            organization.OrganizationId,
                                            false
                                        );

                if (userId < 0)
                { throw new Exception($"Error occured while inserting the Organization User. Email: {model.Email}, UserId: {userId}"); }

                int result = await _teacherClassRepository.SaveTeacherClassMappingsAsync
                                    (
                                        userId,
                                        model.ClassIds?.Split(',')?.Select(Int32.Parse)?.ToArray(),
                                        organizationId,
                                        organizationAdmin
                                    );
                if (result < 0)
                { throw new Exception($"Error occured while updating the Organization Teacher Class Mappings. Email: {model.Email}, UserId: {userId}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationUserController.PostAsync(), DisplayName: {model.DisplayName}, Email: {model.Email}, OrganizationId: {organizationId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/OrganizationUser        
        [HttpPut]
        [SuppressModelStateInvalidFilter]
        public async Task<IActionResult> PutAsync([FromForm] OrganizationUserPutModel model)
        {
            try
            {
                int organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationUserController.PutAsync();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                long organizationAdmin = HttpContext.GetUserId();
                if (organizationAdmin == 0)
                {
                    _logger.LogError($"Error=Incorrect UserId. Source=OrganizationUserController.PutAsync();OrganizationId={organizationId}, UserId={organizationAdmin}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationUserController.PutAsync(), UserId: {model.UserId}");
                    return this.BadRequest();
                }

                var profile = await _profileRepository.GetUserInfoByOrganizationIdAsync(organizationId, model.UserId);
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

                int result = await _teacherClassRepository.SaveTeacherClassMappingsAsync
                                    (
                                        model.UserId,
                                        model.ClassIds?.Split(',')?.Select(Int32.Parse)?.ToArray(),
                                        organizationId,
                                        organizationAdmin
                                    );
                if (result < 0)
                { throw new Exception($"Error occured while updating the Organization Teacher Class Mappings. UserId: {model.UserId}"); }

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
        public async Task<IActionResult> DeleteAsync(long userId)
        {
            try
            {
                int organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationUserController.DeleteAsync();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }
                var profile = await _profileRepository.GetUserInfoByOrganizationIdAsync(organizationId, userId);
                if (profile == null)
                {
                    _logger.LogError($"Profile is null. Source:OrganizationUserController.DeleteAsync(), UserId: {userId}, OrganizationId: {organizationId}");
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

        [HttpGet("[Action]")]
        public async Task<IActionResult> ResetPasswordAsync(string emailId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId))
                    return new StatusCodeResult((int)HttpStatusCode.BadRequest);

                int organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationUserController.ResetPasswordAsync();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                var organization = await _organizationRepository.GetOrganizationAsync(organizationId);

                var user = await _profileRepository.GetUserProfileByEmailIdAsync(emailId);
                // !user.IsSystemCreated - Removed this condition to reset password for all active users.
                // !string.IsNullOrWhiteSpace(user.LoginProvider) - Removed this condition to generate the password even the user doesn't have the LoginProvider.
                if (user == null || user.OrganizationOid != organization.Oid)
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
