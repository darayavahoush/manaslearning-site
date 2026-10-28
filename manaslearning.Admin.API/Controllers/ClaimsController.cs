using manaslearning.Business.Interfaces;
using manaslearning.Helpers;
using manaslearning.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace manaslearning.Admin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IPrivilegeRepository _privilegeRepository;

        public ClaimsController(
            ILogger<ClaimsController> logger,
            IPrivilegeRepository privilegeRepository)
        {
            this._logger = logger;
            this._privilegeRepository = privilegeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var email = this.HttpContext.User.GetEmail();
                if (!email.IsValidEmail())
                    return this.Unauthorized();

                var privileges = await _privilegeRepository.GetUserPrivilegesByEmailAsync(email);
                if (privileges == null)
                    return this.NotFound();

                return Ok(privileges);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ClaimsController.GetAsync");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
