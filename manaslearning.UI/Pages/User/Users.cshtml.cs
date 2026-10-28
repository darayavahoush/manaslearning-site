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
    public class UsersModel : PageModel
    {
        private readonly ILogger _logger;

        private long userId;
        private int organizationId;
        private readonly IStudentClassRepository _studentClassRepository;

        public IEnumerable<StudentClass> studentClasses;
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }

        public UsersModel(
            ILogger<UsersModel> logger,
            IStudentClassRepository studentClassRepository
        )
        {
            _logger = logger;
            _studentClassRepository = studentClassRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                userId = HttpContext.GetUserId();
                organizationId = HttpContext.GetOrganizationId();
                if (userId <= 0) return LocalRedirect(UrlConstants.UNAUTHORIZED);
                studentClasses = await _studentClassRepository.GetStudentClassesByOrganizationIdAsync(organizationId);
                if (studentClasses == null)
                {
                    _logger.LogError("Error=Student Classes not found in database,Source=StudentsModel.OnGetAsync()");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }                
                studentClasses = studentClasses?.OrderBy(s => s.ClassName);
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
                    new BreadcrumbItem{ Title = "Users" }
                },
                IsResponsive = true
            };
        }
    }
}