using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace manaslearning.UI.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }         

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string CurrentEnvironment => _configuration.GetValue<string>("Environment");
        public bool RestrictRegistrations => _configuration.GetValue<bool>("RestrictRegistrations");

        private readonly ILogger<ErrorModel> _logger;
        private readonly IConfiguration _configuration;

        public ErrorModel(ILogger<ErrorModel> logger,
                          IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
