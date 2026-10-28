using manaslearning.Business.Interfaces;
using manaslearning.UI.Helpers;
using manaslearning.Models;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace manaslearning.UI.Controllers.User
{
    [Route("api/u/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger _logger;

        public ProfileController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IProfileRepository profileRepository,
            ILogger<ProfileController> logger)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._profileRepository = profileRepository;
            this._logger = logger;
        }

        // GET: api/u/profile
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                var profile = await _profileRepository.GetUserProfileAsync(_userId);

                if (profile == null)
                {
                    // TODO: show message
                    return NotFound();
                }
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ProfileController.GetAsync;UserId={_userId}");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/<ProfileController>
        [HttpPost]
        public async Task<IActionResult> PostAsync(ProfileUpdateRequestModel model)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();

                if (!IsProfileValid(model))
                    return BadRequest();

                // TODO: validations
                var isProfileUpdated = await _profileRepository.UpdateUserProfileAsync(
                        _userId,
                        model.FirstName,
                        model.LastName,
                        model.DisplayName,
                        model.PhoneNumber,
                        true
                    );

                if (!isProfileUpdated) { return NotFound(); }

                _logger.LogInformation("User profile updated.");

                var profile = await _profileRepository.GetUserProfileAsync(_userId);
                if (profile == null) { return NotFound(); }
                else
                {
                    // Update user claims
                    var user = await _userManager.GetUserAsync(User);
                    var identityResult = await _userManager.ReplaceClaimAsync(user, User.FindFirst(CustomClaimTypes.DisplayName), new Claim(CustomClaimTypes.DisplayName, model.DisplayName));
                    if (identityResult.Succeeded)
                    {
                        await _signInManager.RefreshSignInAsync(user);
                        _logger.LogInformation("User claims updated.");
                    }
                }
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ProfileController.PostAsync;UserId={_userId}");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Validates all the profile inputs
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        private bool IsProfileValid(ProfileUpdateRequestModel model)
        {
            try
            {
                string nameRegex = @"^[\w\s-./]+$";
                if (string.IsNullOrWhiteSpace(model.FirstName) || model.FirstName.Length > 50 || !Regex.IsMatch(model.FirstName, nameRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(5)))
                    return false;

                if (string.IsNullOrWhiteSpace(model.LastName) || model.LastName.Length > 50 || !Regex.IsMatch(model.LastName, nameRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(5)))
                    return false;

                if (string.IsNullOrWhiteSpace(model.DisplayName) || model.DisplayName.Length > 100 || !Regex.IsMatch(model.DisplayName, nameRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(5)))
                    return false;
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.PhoneNumber) || model.PhoneNumber.Length != 10 || !long.TryParse(model.PhoneNumber, out long _phoneNumber))
                return false;

            return true;
        }
    }
}
