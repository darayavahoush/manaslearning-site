using manaslearning.Models;
using manaslearning.T4;
using manaslearning.T4.Templates.Models;
using manaslearning.UI.Helpers.EmailProvider;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITemplateGenerater _templateGenerator;
        private readonly ICustomEmailSender _customEmailSender;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        
        public ResetPasswordConfirmationModel(ILogger<ResetPasswordConfirmationModel> logger,
                                              UserManager<IdentityUser> userManager,
                                              ITemplateGenerater templateGenerater,
                                              ICustomEmailSender customEmailSender,
                                              IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _templateGenerator = templateGenerater;
            _customEmailSender = customEmailSender;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogError("Error=UserId is empty, Source=ResetPasswordConfirmationModel.OnGetAsync.");
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);
                }
                var user = _userManager.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogError($"Error=User not found with id, Source=ResetPasswordConfirmationModel.OnGetAsync. UserId = {id}");
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);
                }

                var passwordUpdateConfirmationModel = await BuildPasswordUpdateConfirmModel(user);
                var htmlContent = _templateGenerator.GetPasswordUpdateConfirmationHtml(passwordUpdateConfirmationModel);
                await _customEmailSender.SendEmailAsync(
                    user.Email,
                    "Password update",
                    htmlContent);
                return Page();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"Error=Exception occurred while sending password reset confirm email. Source=ResetPasswordConfirmationModel.OnGetAsync. UserId = {id}");
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
