using manaslearning.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace manaslearning.UI.UnitTests
{
    public class BaseControllerTest
    {
        protected void SetupIdentity<T>(T controller) where T : ControllerBase
        {
            var identity = new ClaimsIdentity(new List<Claim> { new Claim(CustomClaimTypes.UserId, "1") }, "Cookie");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            var controllerContext = new ControllerContext { HttpContext = httpContext };            
            controller.ControllerContext = controllerContext;
        }
    }
}
