using manaslearning.Business.Interfaces;
using manaslearning.UI.Helpers;
using manaslearning.Models;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;

namespace manaslearning.UI.Pages.User
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger _logger;

        Profile profile;

        public ProfileModel(IProfileRepository profileRepository, ILogger<ProfileModel> logger)
        {
            this._profileRepository = profileRepository;
            this._logger = logger;
        }

        public async void OnGetAsync()
        {
            try
            {
                var userId = HttpContext.GetUserId();
                //if (userId == 0)
                //    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                var profile = await _profileRepository.GetUserProfileAsync(userId);
                if(profile == null)
                {
                    // TODO: show error message
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ProfileModel.OnGetAsync");
            }
        }

        public async void OnPostAsync(ProfileUpdateRequestModel model)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ProfileModel.OnPostAsync");
            }
        }
    }
}