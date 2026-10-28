using manaslearning.Business.Interfaces;
using manaslearning.UI.Helpers;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace manaslearning.UI.Controllers.User
{
    [Route("api/u/[controller]")]
    [ApiController]
    [Authorize]
    public class LicenseController : ControllerBase
    {       
        private readonly ILicenseRepository _licenseRepository;        
        private readonly ILogger _logger;

        public LicenseController(            
            ILicenseRepository licenseRepository,           
            ILogger<LicenseController> logger)
        {           
            this._licenseRepository = licenseRepository;            
            this._logger = logger;
        }

        // GET api/license
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();
                
                var userLicenses = await _licenseRepository.GetOrganizationLicensesInfoAsync(organizationId);
                return Ok(userLicenses.Select(u => new UserLicenseDto
                {
                    AssessmentOid = u.AssessmentOid,
                    AssessmentName = u.AssessmentName,
                    AvailableLicenses = u.AvailableLicenses,
                    UsedLicenses = u.UsedLicenses,
                    InitialPurchaseDate = u.InitialPurchaseDate,
                    RecentPurchanseDate = u.RecentPurchanseDate
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=LicenseController.GetAsync();UserId={_userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }  
    }
}
