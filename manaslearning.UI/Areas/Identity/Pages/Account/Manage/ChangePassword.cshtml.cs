using manaslearning.Models;
using manaslearning.T4;
using manaslearning.T4.Templates.Models;
using manaslearning.UI.Helpers.EmailProvider;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly ITemplateGenerater _templateGenerater;
        private readonly ICustomEmailSender _customEmailSender;
        private readonly IConfiguration _configuration;

        public ChangePasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ChangePasswordModel> logger,
            ITemplateGenerater templateGenerater,
            ICustomEmailSender customEmailSender,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _templateGenerater = templateGenerater;
            _customEmailSender = customEmailSender;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    var userId = _userManager.GetUserId(User);
                    _logger.LogError($"Error = User not found with id. Source = ChangePasswordModel.OnGetAsync(), UserId = {userId}");
                    return NotFound($"Unable to load user with ID '{userId}'.");
                }

                var hasPassword = await _userManager.HasPasswordAsync(user);
                if (!hasPassword)
                {
                    return RedirectToPage("./SetPassword");
                }

                return Page();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"Error = Error occurred on password chnage, Source = ChangePasswordModel.OnGetAsync()");
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

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    var userId = _userManager.GetUserId(User);
                    _logger.LogError($"Error = User not found with id. Source = ChangePasswordModel.OnPostAsync(), UserId = {userId}");
                    return NotFound($"Unable to load user with ID '{userId}'.");
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                await _signInManager.RefreshSignInAsync(user);
                var passwordConfirmModel = await BuildPasswordUpdateConfirmModel(user);
                var htmlContent = _templateGenerater.GetPasswordUpdateConfirmationHtml(passwordConfirmModel);
                await _customEmailSender.SendEmailAsync(user.Email, "Password updated", htmlContent);
                _logger.LogInformation("User changed their password successfully.");
                StatusMessage = "Your password has been changed.";

                return RedirectToPage();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"Error = Error occurred on password chnage, Source = ChangePasswordModel.OnPostAsync()");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private async Task<PasswordUpdateConfirmationModel> BuildPasswordUpdateConfirmModel(IdentityUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var displayName = claims.FirstOrDefault(c => c.Type == CustomClaimTypes.DisplayName).Value;
            displayName = string.IsNullOrWhiteSpace(displayName) ? user.Email : displayName;
            var passwordConfirmModel = new PasswordUpdateConfirmationModel()
            {
                Name = displayName,
                UserName = user.UserName,
                Company = new Company()
            };
            passwordConfirmModel.Company.CompanyAddress = new T4.Templates.Models.Address();
            _configuration.GetSection("Company:Info").Bind(passwordConfirmModel.Company);
            _configuration.GetSection("Company:CompanyAddress").Bind(passwordConfirmModel.Company.CompanyAddress);
            return passwordConfirmModel;
        }
    }
}
