using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User.Subscription
{
    [Authorize]
    public class NewSubscriptionModel : PageModel
    {
        private readonly ILogger<NewSubscriptionModel> _logger;
        private readonly IItemRepository _itemRepository;

        private long _userId;
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }
        public ItemInfo Item { get; set; }

        public NewSubscriptionModel(ILogger<NewSubscriptionModel> logger, IItemRepository itemRepository)
        {
            _logger = logger;
            _itemRepository = itemRepository;
        }

        public async Task<IActionResult> OnGetAsync(Guid itemOid)
        {
            try
            {
                _userId = HttpContext.GetUserId();
                if (_userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                if(itemOid.Equals(Guid.Empty))
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);

                this.Item = await _itemRepository.GetItemInfoByOidAsync(itemOid);
                if (this.Item == null)
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);

                // check licenses available
                // if no, show purchase section
                // if yes
                //   show existing license and give option to buy more license
                //   or redirect to manage subscription page

                SetBreadcrumb(this.Item.AssessmentName);

                return this.Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=Subscription.NewSubscriptionModel.OnGetAsync(), UserId={_userId}, ItemOid={itemOid}");
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
