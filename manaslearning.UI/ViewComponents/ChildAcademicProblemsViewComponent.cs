using manaslearning.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace manaslearning.UI.ViewComponents
{
    public class ChildAcademicProblemsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(bool showHeader = true)
        {
            var viewModel = new ChildAcademicProblemsViewModel
            {
                IsHeaderVisible = showHeader
            };
            return View(viewModel);
        }
    }
}
