using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User
{
    [Authorize]
    public class StudentClassesModel : PageModel
    {
        private readonly ILogger _logger;

        private long userId;        
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }

        public StudentClassesModel(
            ILogger<StudentClassesModel> logger            
        )
        {
            _logger = logger;           
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                userId = HttpContext.GetUserId();
                if (userId <= 0) return LocalRedirect(UrlConstants.UNAUTHORIZED);                
                SetBreadcrumb();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error: {ex.Message}, UserId={userId}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private void SetBreadcrumb()
        {
            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = new BreadcrumbItem[] {
                    new BreadcrumbItem{ Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                    new BreadcrumbItem{ Title = "Classes" }
                },
                IsResponsive = true
            };
        }
    }
}