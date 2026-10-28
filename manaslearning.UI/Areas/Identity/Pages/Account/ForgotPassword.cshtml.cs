using manaslearning.Models;
using manaslearning.T4;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
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

namespace manaslearning.UI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ITemplateGenerater _templateGenerater;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager,
                                   IEmailSender emailSender,
                                   ITemplateGenerater templateGenerater,
                                   ILogger<ForgotPasswordModel> logger,
                                   IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _templateGenerater = templateGenerater;
            _logger = logger;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                    {
                        // Don't reveal that the user does not exist or is not confirmed
                        return RedirectToPage("./ForgotPasswordConfirmation");
                    }

                    // For more information on how to enable account confirmation and password reset please 
                    // visit https://go.microsoft.com/fwlink/?LinkID=532713
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ResetPassword",
                        pageHandler: null,
                        values: new { area = "Identity", code },
                        protocol: Request.Scheme);
                    var resetPasswordModel = await BuildResetPasswordModel(user, HtmlEncoder.Default.Encode(callbackUrl));
                    var htmlContent = _templateGenerater.GetResetPasswordHtml(resetPasswordModel);

                    await _emailSender.SendEmailAsync(
                        Input.Email,
                        "Reset Password",
                        htmlContent);

                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=ForgotPasswordModel.OnPostAsync");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private async Task<T4.Templates.Models.ResetPasswordModel> BuildResetPasswordModel(IdentityUser user, string resetPasswordUrl)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var displayName = claims.FirstOrDefault(c => c.Type == CustomClaimTypes.DisplayName).Value;
            displayName = string.IsNullOrWhiteSpace(displayName) ? user.Email : displayName;
            var emailVerificationModel = new T4.Templates.Models.ResetPasswordModel()
            {
                Name = displayName,
                ResetPasswordUrl = resetPasswordUrl,
                Company = new T4.Templates.Models.Company()
            };
            emailVerificationModel.Company.CompanyAddress = new T4.Templates.Models.Address();
            _configuration.GetSection("Company:Info").Bind(emailVerificationModel.Company);
            _configuration.GetSection("Company:CompanyAddress").Bind(emailVerificationModel.Company.CompanyAddress);
            return emailVerificationModel;
        }
    }
}
