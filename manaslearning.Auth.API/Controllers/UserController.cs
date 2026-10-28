using Google.Apis.Auth;
using manaslearning.Auth.API.Helpers.EmailProvider;
using manaslearning.Auth.API.Models;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.T4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace manaslearning.Auth.API.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly JwtGenerator _jwtGenerator;
    private readonly ILogger _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IProfileRepository _profileRepository;
    private readonly ITemplateGenerater _templateGenerater;
    private readonly ICustomEmailSender _emailSender;

    public UserController(
        IConfiguration configuration,
        ILogger<UserController> logger,
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IProfileRepository profileRepository,
        ITemplateGenerater templateGenerater,
        ICustomEmailSender emailSender)
    {
        _logger = logger;
        _configuration = configuration;
        _signInManager = signInManager;
        _userManager = userManager;
        _profileRepository = profileRepository;
        _templateGenerater = templateGenerater;
        _jwtGenerator = new JwtGenerator(configuration.GetValue<string>("Jwt:Keys:Private"));
        _emailSender = emailSender;
    }

    [AllowAnonymous]
    [HttpPost("[Action]")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest authenticateRequest)
    {
        try
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();

            var clientId = _configuration.GetValue<string>("Authentication:Google:ClientId");
            settings.Audience = new List<string> { clientId };

            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(authenticateRequest.IdToken, settings);

            var user = await _userManager.FindByLoginAsync("google", payload.Subject);

            if (user == null)
                return Unauthorized("User is not exists. Please register.");

            if (!user.EmailConfirmed)
                return Unauthorized("Email verification pending.");

            if (user.Email?.ToLower() != authenticateRequest.EmailId.ToLower())
            {
                _logger.LogError($"Incorrect EmailId from the request. Source=UserController.AuthenticateAsync(), EmailId: {authenticateRequest.EmailId}");
                return BadRequest("Invalid EmailId");
            }


            return Ok(new { token = _jwtGenerator.CreateUserAuthToken(user.Email) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}. Source=UserController.AuthenticateAsync()");
            return this.StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpPost("[Action]")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        try
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();

            var clientId = _configuration.GetValue<string>("Authentication:Google:ClientId");
            settings.Audience = new List<string> { clientId };

            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(registerRequest.IdToken, settings);

            var user = await _userManager.FindByLoginAsync("google", payload.Subject);

            if (user != null)
                return BadRequest("User already registered");

            user = await _userManager.FindByEmailAsync(registerRequest.EmailId);
            if (user == null)
            {
                // no user exists with this email
                user = new IdentityUser
                {
                    Email = registerRequest.EmailId,
                    UserName = registerRequest.EmailId,
                    EmailConfirmed = false,
                };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await CreateUserProfile(user, payload.GivenName, payload.FamilyName, payload.Name);

                    // link the user to this login
                    var info = new UserLoginInfo("google", payload.Subject, "google".ToUpperInvariant());
                    result = await _userManager.AddLoginAsync(user, info);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Erorr while adding login, EmailId:{registerRequest.EmailId}, Errors: {string.Join(',', result.Errors)}");
                    }
                    _logger.LogInformation($"User {registerRequest.EmailId} created an account using {info.LoginProvider} provider.");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var website = _configuration.GetValue<string>("Website");
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = $"{website}/account/confirm-email?userid={user.Id}&code={code}&version=6"; // Version is hardcoded
                    var signupModel = await BuildSignupModel(HtmlEncoder.Default.Encode(callbackUrl), user.Email);
                    var signupHtml = _templateGenerater.GetSignUpHtml(signupModel);
                    await _emailSender.SendEmailAsync("Confirm your email", signupHtml, user.Email, string.Empty, string.Empty);
                }
            }
            return Ok("User successfuly registerd. Please confirm the email.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}. Source=UserController.RegisterAsync()");
            return this.StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpPost("[Action]")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ConfirmationEmailRequest confirmationEmailRequest)
    {
        try
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();

            var clientId = _configuration.GetValue<string>("Authentication:Google:ClientId");
            settings.Audience = new List<string> { clientId };

            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(confirmationEmailRequest.IdToken, settings);

            var user = await _userManager.FindByLoginAsync("google", payload.Subject);

            if (user == null)
                return BadRequest("User is not registered");

            if (user.EmailConfirmed)
                return BadRequest("User is already confirmed");

            var website = _configuration.GetValue<string>("Website");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = $"{website}/account/confirm-email?userid={user.Id}&code={code}&version=6"; // Version is hardcoded
            var signupModel = await BuildSignupModel(HtmlEncoder.Default.Encode(callbackUrl), user.Email);
            var signupHtml = _templateGenerater.GetSignUpHtml(signupModel);
            await _emailSender.SendEmailAsync("Confirm your email", signupHtml, user.Email, string.Empty, string.Empty);

            return Ok("Confirmation email successfully sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}. Source=UserController.ResendConfirmationEmailAsync()");
            return this.StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpPost("[Action]")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest confirmEmailRequest)
    {
        try
        {
            if (confirmEmailRequest.UserId == null || confirmEmailRequest.Code == null)
            {
                return BadRequest("Incorrect UserId/Code.");
            }
            var user = await _userManager.FindByIdAsync(confirmEmailRequest.UserId);
            if (user.EmailConfirmed)
                return BadRequest("User is already confirmed");

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmEmailRequest.Code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                _logger.LogError($"Error occured while confirming the email. Source=UserController.ConfirmEmailAsync(), UserId:{confirmEmailRequest.UserId}");
                throw new Exception($"Error occured while confirming the email.UserId:{confirmEmailRequest.UserId}");
            }

            return Ok("Email has been confirmed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}. Source=UserController.ConfirmEmailAsync()");
            return this.StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }

    private async Task<T4.Templates.Models.SignupModel> BuildSignupModel(string callbackUrl, string emailId)
    {
        var displayName = emailId;
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

    private async Task CreateUserProfile(IdentityUser user, string firstName, string lastName, string displayName)
    {
        var id = await _profileRepository.CreateUserIdAsync(user.Id, user.Email);

        var isProfileCreated = await _profileRepository.CreateUserProfileAsync(
                                id,
                                firstName,
                                lastName,
                                displayName,
                                user.Email
                                );
        if (!isProfileCreated)
        {
            throw new Exception("Error occured during register.");
        }

        var profile = await _profileRepository.GetUserProfileAsync(id);
        if (profile.OrganizationId <= 0)
        {
            throw new Exception($"Error occured durin register. Incorrect organizationId. UserId: {id}");
        }

        // If they exist, add claims to the user for:
        // UserId
        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.UserId, $"{id}"));
        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.DisplayName, displayName));
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.OrganizationId, $"{profile.OrganizationId}"));
        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.LoginType, $"{profile.LoginType}"));
    }
}

public class JwtGenerator
{
    readonly RsaSecurityKey _key;

    public JwtGenerator(string signingKey)
    {
        RSA privateRSA = RSA.Create();
        privateRSA.ImportRSAPrivateKey(Convert.FromBase64String(signingKey), out _);
        _key = new RsaSecurityKey(privateRSA);
    }

    public string CreateUserAuthToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescritor = new SecurityTokenDescriptor
        {
            Audience = "myApi",
            Issuer = "AuthService",
            Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Sid, userId) }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.RsaSha512) // SecurityAlgorithms.RsaSha256
        };

        var token = tokenHandler.CreateToken(tokenDescritor);
        return tokenHandler.WriteToken(token);
    }
}
