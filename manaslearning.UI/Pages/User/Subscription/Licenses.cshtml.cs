using manaslearning.Business.Interfaces;
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

namespace manaslearning.UI.Pages.User.Subscription
{
    [Authorize]
    public class LicensesModel : PageModel
    {
        private readonly ILogger _logger;
        private readonly ILicenseRepository _licenseRepository;
        private readonly IItemRepository _itemRepository;        

        public IEnumerable<LicenseModel> Licenses { get; set; }
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }

        private long _userId;

        public LicensesModel(
            ILogger<LicensesModel> logger,
            ILicenseRepository licenseRepository,
            IItemRepository itemRepository)
        {
            this._logger = logger;
            this._licenseRepository = licenseRepository;
            this._itemRepository = itemRepository;            
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _userId = HttpContext.GetUserId();                
                int organizationId = HttpContext.GetOrganizationId();
                if (_userId == 0 || organizationId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                SetBreadcrumb();

                var licenses = await _licenseRepository.GetOrganizationLicensesInfoAsync(organizationId);

                var items = await _itemRepository.GetItemsInfoAsync();
                if (items == null)
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);

                this.Licenses = licenses?.Select(u => new LicenseModel
                {
                    AssessmentOid = u.AssessmentOid,
                    ItemOid = items?.FirstOrDefault(x => x.AssessmentId == u.AssessmentId)?.Oid,
                    AssessmentName = u.AssessmentName,
                    AvailableLicenses = u.AvailableLicenses,
                    UsedLicenses = u.UsedLicenses,
                    InitialPurchaseDate = u.InitialPurchaseDate,
                    RecentPurchanseDate = u.RecentPurchanseDate
                });

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=Subscription.LicensesModel.OnGetAsync(), UserId={_userId}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private void SetBreadcrumb()
        {
            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = new BreadcrumbItem[] {
                    new BreadcrumbItem{ Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                    new BreadcrumbItem{ Title = "Licenses" }
                },
                IsResponsive = true
            };
        }
    }

    public class LicenseModel
    {
        public Guid AssessmentOid { get; set; }
        public Guid? ItemOid { get; set; }
        public string AssessmentName { get; set; }
        public int AvailableLicenses { get; set; }
        public int UsedLicenses { get; set; }
        public DateTime InitialPurchaseDate { get; set; }
        public DateTime RecentPurchanseDate { get; set; }
    }
}
