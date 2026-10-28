using Microsoft.AspNetCore.Mvc;

namespace manaslearning.UI.Controllers
{
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Healthy";
        }
    }
}
