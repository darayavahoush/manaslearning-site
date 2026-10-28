using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System;
using System.Net.Http.Headers;

namespace manaslearning.UI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [TempData]
        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public bool UserEmailWasConfirmedBefore { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code, decimal? version)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }
            if (user.EmailConfirmed)
            {
                UserEmailWasConfirmedBefore = true;
                return Page();
            }
            if (version != null)
            {
                HttpClient client = new HttpClient();
                var authApi = _configuration.GetValue<string>("AuthApi");
                client.BaseAddress = new Uri(authApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var request = new { UserId = userId, Code = code };
                HttpResponseMessage response = await client.PostAsJsonAsync(
                    "User/ConfirmEmail", request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    IsEmailConfirmed = true;
                }
            }
            else {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await _userManager.ConfirmEmailAsync(user, code);
                IsEmailConfirmed = result.Succeeded;
            }            
            return Page();
        }        
    }
}
