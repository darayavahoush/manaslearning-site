using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace manaslearning.UI.Pages
{
    public class ContactModel : PageModel
    {
        public string ReCaptchaSiteKey { get; }

        public ContactModel(IConfiguration configuration)
        {
            ReCaptchaSiteKey = configuration.GetValue<string>("reCaptcha:SiteKey");
        }

        public void OnGet()
        {

        }
    }
}