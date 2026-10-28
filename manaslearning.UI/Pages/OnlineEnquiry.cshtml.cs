using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace manaslearning.UI.Pages
{
    public class OnlineEnquiryModel : PageModel
    {
        public string ReCaptchaSiteKey { get; }

        public OnlineEnquiryModel(IConfiguration configuration)
        {
            ReCaptchaSiteKey = configuration.GetValue<string>("reCaptcha:SiteKey");
        }

        public void OnGet()
        {

        }
    }
}
