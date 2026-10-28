using manaslearning.Models;
using manaslearning.T4;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace manaslearning.UI.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ITemplateGenerater _templateGenerater;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public EmailModel(ILogger<EmailModel> logger,
                          UserManager<IdentityUser> userManager,
                          SignInManager<IdentityUser> signInManager,
                          IEmailSender emailSender,
                          ITemplateGenerater templateGenerater,
                          IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _templateGenerater = templateGenerater;
            _configuration = configuration;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    var userId = _userManager.GetUserId(User);
                    _logger.LogError($"Error = User not found with id. Source = EmailModel.OnPostChangeEmailAsync(), UserId = {userId}");
                    return NotFound($"Unable to load user with ID '{userId}'.");
                }

                if (!ModelState.IsValid)
                {
                    await LoadAsync(user);
                    return Page();
                }

                var email = await _userManager.GetEmailAsync(user);
                if (Input.NewEmail != email)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmailChange",
                        pageHandler: null,
                        values: new { userId = userId, email = Input.NewEmail, code = code },
                        protocol: Request.Scheme);
                    var confirmEmailChangeModel = await BuildConfirmEmailChangeModel(user, Input.NewEmail, email, callbackUrl);
                    var htmlContent = _templateGenerater.GetConfirmEmailChnageHtml(confirmEmailChangeModel);
                    await _emailSender.SendEmailAsync(
                        Input.NewEmail,
                        "Confirm your email",
                        htmlContent);

                    StatusMessage = "Confirmation link to change email sent. Please check your email.";
                    return RedirectToPage();
                }

                StatusMessage = "Your email is unchanged.";
                return RedirectToPage();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"Error = Error occurred on email chnage, Source = EmailModel.OnPostChangeEmailAsync()");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }

        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            try

            {
                var user = await _userManager.GetUserAsync(User);
                var userId = await _userManager.GetUserIdAsync(user);
                if (user == null)
                {
                    _logger.LogError($"Error = User not found with id. Source = EmailModel.OnPostSendVerificationEmailAsync(), UserId = {userId}");
                    return NotFound($"Unable to load user with ID '{userId}'.");
                }

                if (!ModelState.IsValid)
                {
                    await LoadAsync(user);
                    return Page();
                }

                var email = await _userManager.GetEmailAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                var claims = await _userManager.GetClaimsAsync(user);
                var displayName = claims.FirstOrDefault(c => c.Type == CustomClaimTypes.DisplayName).Value;
                displayName = string.IsNullOrWhiteSpace(displayName) ? user.Email : displayName;
                var emailVerificationModel = await BuildEmailVerificationModel(user, HtmlEncoder.Default.Encode(callbackUrl));
                var htmlContent = _templateGenerater.GetVerificationEmailHtml(emailVerificationModel);
                await _emailSender.SendEmailAsync(email, "Confirm your email", htmlContent);

                StatusMessage = "Verification email sent. Please check your email.";
                return RedirectToPage();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"Error = Error occurred on email chnage, Source = EmailModel.OnPostSendVerificationEmailAsync()");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private async Task<T4.Templates.Models.ConfirmEmailChangeModel> BuildConfirmEmailChangeModel(IdentityUser user, string newEmail, string oldEmail, string confirmUrl)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var displayName = claims.FirstOrDefault(c => c.Type == CustomClaimTypes.DisplayName).Value;
            displayName = string.IsNullOrWhiteSpace(displayName) ? user.Email : displayName;
            var confirmEmailChangeModel = new T4.Templates.Models.ConfirmEmailChangeModel()
            {
                Name = displayName,
                NewEmail = newEmail,
                OldEmail = oldEmail,
                ConfirmUrl = confirmUrl,
                Company = new T4.Templates.Models.Company()
            };
            confirmEmailChangeModel.Company.CompanyAddress = new T4.Templates.Models.Address();
            _configuration.GetSection("Company:Info").Bind(confirmEmailChangeModel.Company);
            _configuration.GetSection("Company:CompanyAddress").Bind(confirmEmailChangeModel.Company.CompanyAddress);
            return confirmEmailChangeModel;
        }

        private async Task<T4.Templates.Models.EmailVerificationModel> BuildEmailVerificationModel(IdentityUser user, string confirmUrl)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var displayName = claims.FirstOrDefault(c => c.Type == CustomClaimTypes.DisplayName).Value;
            displayName = string.IsNullOrWhiteSpace(displayName) ? user.Email : displayName;
            var emailVerificationModel = new T4.Templates.Models.EmailVerificationModel()
            {
                Name = displayName,
                ActivationUrl = confirmUrl,
                Company = new T4.Templates.Models.Company()
            };
            emailVerificationModel.Company.CompanyAddress = new T4.Templates.Models.Address();
            _configuration.GetSection("Company:Info").Bind(emailVerificationModel.Company);
            _configuration.GetSection("Company:CompanyAddress").Bind(emailVerificationModel.Company.CompanyAddress);
            return emailVerificationModel;
        }

    }
}
