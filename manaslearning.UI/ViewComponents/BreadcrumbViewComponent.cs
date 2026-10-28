using manaslearning.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace manaslearning.UI.ViewComponents
{
    public class BreadcrumbViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(BreadcrumbItem[] items, bool showBreadcrumb = true, bool isResponsive = false)
        {
            var viewModel = new BreadcrumbViewModel
            {
                ShowBreadcrumb = showBreadcrumb,
                Items = items,
                IsResponsive = isResponsive
            };
            return View(viewModel);
        }
    }
}
