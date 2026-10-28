using manaslearning.Admin.UI.Models;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace manaslearning.Admin.UI.Pages
{
    public class ParentComponentBase : ComponentBase
    {
        [Inject] protected IConfiguration Configuration { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }

        protected void HandleFailedResonse(ApiResponseModel apiResponseModel, string title, string contentTitle, string message)
        {
            if (apiResponseModel == null)
                return;
            else if (apiResponseModel.StatusCode == HttpStatusCode.Unauthorized)
                NavigationManager.NavigateTo("/login", true);
            else
            {
                // TODO: implement model service
            }
        }
    }
}
