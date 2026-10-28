using manaslearning.UI.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace manaslearning.UI.Pages
{
    public class ConsultModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ConsultModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ConsultationSettings Settings { get; private set; }

        public void OnGet()
        {
            Settings = ConsultationSettings.From(_configuration);
        }
    }
}
