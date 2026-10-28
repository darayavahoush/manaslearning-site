using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using manaslearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace manaslearning.UI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ConfirmEmailChangeModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }
            
            //TODO:Commented for now, we are getting decoded code only
            //code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                StatusMessage = "Error changing email.";
                return Page();
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                StatusMessage = "Error changing user name.";
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            var updateClaimResult = await UpdateUserClaims(user, email);
            if (!updateClaimResult)
            {
                return Page();
            }
            StatusMessage = "Thank you for confirming your email change.";
            return Page();
        }

        private async Task<bool> UpdateUserClaims(IdentityUser user, string email)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var displayNameClaim = new List<Claim>(userClaims).FirstOrDefault(c => c.Type == CustomClaimTypes.DisplayName);

            if (displayNameClaim != null)
            {
                var claimRemoveResult = await _userManager.ReplaceClaimAsync(user, displayNameClaim, new Claim(CustomClaimTypes.DisplayName, email));

                if (!claimRemoveResult.Succeeded)
                {
                    StatusMessage = "Error occured while updating display name for user";
                    return false;
                }
            }
            var emailClaim = new List<Claim>(userClaims).FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim != null)
            {
                var claimRemoveResult = await _userManager.ReplaceClaimAsync(user, emailClaim, new Claim(ClaimTypes.Email, email));
                if (!claimRemoveResult.Succeeded)
                {
                    StatusMessage = "Error occured while updating email of user";
                    return false;
                }
            }
            return true;
        }
    }
}
