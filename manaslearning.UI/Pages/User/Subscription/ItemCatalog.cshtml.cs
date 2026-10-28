using manaslearning.Business.Interfaces;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User.Subscription
{
    [Authorize]
    public class ItemCatalogModel : PageModel
    {
        private readonly ILogger _logger;
        private readonly IItemRepository _itemRepository;
        private readonly ILicenseRepository _licenseRepository;        

        private long _userId;

        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }
        public IEnumerable<ItemDto> Items { get; set; }

        public ItemCatalogModel(
            ILogger<ItemCatalogModel> logger,
            IItemRepository itemRepository,
            ILicenseRepository licenseRepository)
        {
            _logger = logger;
            _itemRepository = itemRepository;
            _licenseRepository = licenseRepository;            
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();
                if (_userId == 0 || organizationId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                var items = await _itemRepository.GetItemsInfoAsync();
                if (items == null)
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);                

                var userLicenses = await _licenseRepository.GetOrganizationLicensesAsync(organizationId);

                Items = items.Select(a => new ItemDto
                {
                    Oid = a.Oid,
                    Discount = a.Discount,
                    GSTAmount = a.GSTAmount,
                    GSTRate = a.GSTRate,
                    ItemDescription = a.ItemDescription,
                    ItemPrice = a.ItemPrice,
                    MaximumDiscount = a.MaximumDiscount,
                    AssessmentId = a.AssessmentId,
                    AssessmentCode = a.AssessmentCode,
                    AssessmentDescription = a.AssessmentDescription,
                    AssessmentName = a.AssessmentName,
                    HasLicense = userLicenses != null && userLicenses.Any(x => x.AssessmentId == a.AssessmentId)
                });

                SetBreadcrumb();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=Subscription.ItemCatalogModel.OnGetAsync(), UserId={_userId}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private void SetBreadcrumb()
        {
            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = new BreadcrumbItem[] {
                    new BreadcrumbItem{ Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                    new BreadcrumbItem{ Title = "Purchase services" }
                },
                IsResponsive = true
            };
        }
    }
}
