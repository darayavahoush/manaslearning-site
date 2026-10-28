using manaslearning.Admin.API.Helpers;
using manaslearning.Admin.API.Models;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace manaslearning.Admin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IProfileRepository _profileRepository;

        public UserController(
            ILogger<UserController> logger,
            IProfileRepository profileRepository
            )
        {
            _logger = logger;
            _profileRepository = profileRepository;
        }

        [AllowedPrivileges(PrivilegeTypes.Users_Read)]
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var users = await _profileRepository.GetUsersInfoAsync();
                if (users == null || !users.Any())
                    return NotFound();

                return Ok(users
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .Select(u => new UserDto
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
                _logger.LogError(ex, $"{ex.Message}. Source=UserController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> ResetPasswordAsync(string emailId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId))
                    return new StatusCodeResult((int)HttpStatusCode.BadRequest);

                var user = await _profileRepository.GetUserProfileByEmailIdAsync(emailId);
                if (user == null || !user.IsSystemCreated || !string.IsNullOrWhiteSpace(user.LoginProvider))
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
