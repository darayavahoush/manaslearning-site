using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace manaslearning.UI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger _logger;
        public ResetPasswordModel(ILogger<ResetPasswordModel> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string code = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    _logger.LogError("Error=Password reset code is null or empty, Source=ResetPasswordModel.OnGetAsync");
                    return BadRequest("A code must be supplied for password reset.");
                }
                else
                {
                    Input = new InputModel
                    {
                        Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                    };
                    return Page();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"Error=Error occurred on reset password, Source=ResetPasswordModel.OnGetAsync");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
                if (result.Succeeded)
                {
                    return RedirectToPage("./ResetPasswordConfirmation", new { id = user.Id });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"Error=Error occurred on reset password, Source=ResetPasswordModel.OnPostAsync");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }

        }
    }
}
