using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace manaslearning.UI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly IProfileRepository _profileRepository;
        private readonly IConfiguration _configuration;

        public ExternalLoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender,
            IProfileRepository profileRepository,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _profileRepository = profileRepository;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        public bool RestrictRegistrations => _configuration.GetValue<bool>("RestrictRegistrations");

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);

                #region Custom

                if (returnUrl == "/")
                    returnUrl = UrlConstants.USER_DASHBOARD;

                #endregion Custom

                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout"); // TODO: Verify this workflow
            }
            else
            {
                if (_configuration.GetValue<bool?>("RestrictRegistrations") == true)
                {
                    _logger.LogError($"Registrations are disabled in the current environment.");
                    return LocalRedirect(UrlConstants.ERROR_PAGE);
                }
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }

        //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-5.0&tabs=visual-studio#debug
        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                #region Custom

                // Check user has already an account
                var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

                IdentityUser user = null;
                if (signInResult.IsNotAllowed)
                {
                    user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                    if (!user.EmailConfirmed)
                    {
                        // redirect to email confirmation page
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }
                    }
                }

                // TODO:
                // Implement result.IsLockedOut
                // Implement result.RequiresTwoFactor

                #endregion Custom

                user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        #region Custom

                        var id = await _profileRepository.CreateUserIdAsync(user.Id, user.Email);
                        var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
                        var displayName = info.Principal.FindFirstValue(ClaimTypes.Name);

                        var isProfileCreated = await _profileRepository.CreateUserProfileAsync(
                                                id,
                                                firstName,
                                                lastName,
                                                displayName,
                                                user.Email
                                                );

                        if (!isProfileCreated)
                        {
                            ErrorMessage = "Error occured during register.";
                            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
                        }

                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var profile = await _profileRepository.GetUserProfileAsync(id);
                        if (profile.OrganizationId <= 0)
                        {
                            ErrorMessage = "Error occured during register.";
                            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
                        }

                        #region User Claims

                        // If they exist, add claims to the user for:
                        // UserId
                        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.UserId, $"{id}"));

                        // OrganizationId
                        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.OrganizationId, $"{profile.OrganizationId}"));

                        // LoginType
                        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.LoginType, $"{profile.LoginType}"));

                        // Name (Display Name)
                        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                        {
                            await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.DisplayName, info.Principal.FindFirst(ClaimTypes.Name).Value));
                        }

                        // Email
                        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                        {
                            await _userManager.AddClaimAsync(user, info.Principal.FindFirst(ClaimTypes.Email));
                        }

                        // Locale
                        //if (info.Principal.HasClaim(c => c.Type == "urn:google:locale"))
                        //{
                        //    await _userManager.AddClaimAsync(user, info.Principal.FindFirst("urn:google:locale"));
                        //}

                        // Picture
                        //if (info.Principal.HasClaim(c => c.Type == "urn:google:picture"))
                        //{
                        //    await _userManager.AddClaimAsync(user, info.Principal.FindFirst("urn:google:picture"));
                        //}

                        #endregion User Claims

                        #endregion Custom

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        #region Custom

                        await _signInManager.RefreshSignInAsync(user);

                        if (returnUrl == "/")
                            returnUrl = UrlConstants.USER_DASHBOARD;

                        #endregion Custom

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                        ModelState.AddModelError(string.Empty, "The email address is already registered, please login with your credentials or use recovery credentials to log in.");
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
