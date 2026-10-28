using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.T4;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace manaslearning.UI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IProfileRepository _profileRepository;
        private readonly ITemplateGenerater _templateGenerater;
        private readonly IConfiguration _configuration;
        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IProfileRepository profileRepository,
            ITemplateGenerater templateGenerater,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _profileRepository = profileRepository;
            _templateGenerater = templateGenerater;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Enter a First Name")]
            [Display(Name = "First Name")]
            [StringLength(50, ErrorMessage = "Max. 50 characters allowed.")]
            [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Accepts only alphabet")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Enter a Last Name")]
            [Display(Name = "Last Name")]
            [StringLength(50, ErrorMessage = "Max. 50 characters allowed.")]
            [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Accepts only alphabet")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Enter a Display Name")]
            [Display(Name = "Display Name")]
            [StringLength(100, ErrorMessage = "Max. 100 characters allowed.")]
            [RegularExpression("^[a-zA-Z ,]+$", ErrorMessage = "Accepts only alphabet. sample is \"FirstName, LastName\"")]
            public string DisplayName { get; set; }

            [Required(ErrorMessage = "Enter an Email")]
            [EmailAddress(ErrorMessage = "Enter a valid email")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Enter a Password")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (_configuration.GetValue<bool?>("RestrictRegistrations") == true)
            {
                _logger.LogError($"Registrations are disabled in the current environment.");
                return LocalRedirect(UrlConstants.ERROR_PAGE);
            }
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await CreateUserProfile(user);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    var signupModel = await BuildSignupModel(HtmlEncoder.Default.Encode(callbackUrl));
                    var signupHtml = _templateGenerater.GetSignUpHtml(signupModel);
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email", signupHtml);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        await _signInManager.RefreshSignInAsync(user);
                        if (returnUrl == "/")
                            returnUrl = UrlConstants.USER_DASHBOARD;
                        return LocalRedirect(returnUrl);
                    }

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<T4.Templates.Models.SignupModel> BuildSignupModel(string callbackUrl)
        {
            var displayName = string.IsNullOrWhiteSpace(Input.DisplayName) ? Input.Email : Input.DisplayName;
            var signupModel = new T4.Templates.Models.SignupModel()
            {
                Name = displayName,
                ActivationUrl = callbackUrl,
                Company = new T4.Templates.Models.Company()
            };
            signupModel.Company.CompanyAddress = new T4.Templates.Models.Address();
            _configuration.GetSection("Company:Info").Bind(signupModel.Company);
            _configuration.GetSection("Company:CompanyAddress").Bind(signupModel.Company.CompanyAddress);
            return signupModel;
        }

        private async Task CreateUserProfile(IdentityUser user)
        {
            var id = await _profileRepository.CreateUserIdAsync(user.Id, user.Email);

            var isProfileCreated = await _profileRepository.CreateUserProfileAsync(
                                    id,
                                    Input.FirstName,
                                    Input.LastName,
                                    Input.DisplayName,
                                    user.Email
                                    );
            if (!isProfileCreated)
            {
                ModelState.AddModelError(string.Empty, "Error occured during register.");
                return;
            }
            var profile = await _profileRepository.GetUserProfileAsync(id);
            if (profile.OrganizationId <= 0)
            {
                ModelState.AddModelError(string.Empty, $"Error occured durin register. Incorrect organizationId. UserId: {id}");
                return;
            }

            // If they exist, add claims to the user for:
            // UserId
            await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.UserId, $"{id}"));
            await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.DisplayName, Input.DisplayName));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, Input.Email));
            await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.OrganizationId, $"{profile.OrganizationId}"));


        }
    }
}
