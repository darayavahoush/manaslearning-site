using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;

namespace manaslearning.UI.Pages.User.Subscription
{
    [Authorize]
    public class ManageSubscriptionModel : PageModel
    {
        private readonly ILogger<ManageSubscriptionModel> _logger;

        private long userId;
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }

        public ManageSubscriptionModel(ILogger<ManageSubscriptionModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet(Guid itemOid)
        {
            try
            {
                userId = HttpContext.GetUserId();
                if (userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                // check licenses available
                // if no, show purchase section
                // if yes, show existing license and give option to buy more license

                var itemName = "ADHD Assessment"; // TODO: assign this
                SetBreadcrumb(itemName);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=Subscription.ManageSubscriptionModel.OnGetAsync(), UserId={userId}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private void SetBreadcrumb(string itemName)
        {
            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = new BreadcrumbItem[] {
                    new BreadcrumbItem{ Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                    new BreadcrumbItem{ Title = "Purchase services", Href = UrlConstants.USER_PURCHASE_SERVICES },
                    new BreadcrumbItem{ Title = $"{itemName}" }
                },
                IsResponsive = true
            };
        }
    }
}
