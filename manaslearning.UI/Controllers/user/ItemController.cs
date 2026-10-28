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
    public class ItemController : ControllerBase
    {       
        private readonly IItemRepository _itemRepository;
        private readonly ILicenseRepository _licenseRepository;        
        private readonly ILogger _logger;

        public ItemController(            
            IItemRepository itemRepository,
            ILicenseRepository licenseRepository,           
            ILogger<ItemController> logger)
        {           
            this._itemRepository = itemRepository;
            this._licenseRepository = licenseRepository;
            this._logger = logger;
        }

        // GET api/item
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            long _userId = 0;           
            try
            {
                _userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();                
                var licenses = await _licenseRepository.GetOrganizationLicensesAsync(organizationId);
                var items = await _itemRepository.GetItemsInfoAsync();
                return Ok(items.Select(i => new ItemDto
                {
                    Oid = i.Oid,
                    Discount = i.Discount,
                    GSTAmount = i.GSTAmount,
                    GSTRate = i.GSTRate,
                    ItemDescription = i.ItemDescription,
                    ItemPrice = i.ItemPrice,
                    MaximumDiscount = i.MaximumDiscount,
                    AssessmentCode = i.AssessmentCode,
                    AssessmentDescription = i.AssessmentDescription,
                    AssessmentName = i.AssessmentName,
                    HasLicense = licenses.Any(ul => ul.AssessmentId == i.AssessmentId)
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ItemController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        // GET api/Item?Oid={oid}
        [HttpGet("{oid:Guid}")]
        public async Task<IActionResult> GetAsync(Guid oid)
        {            
            try
            {
                var model = await _itemRepository.GetItemInfoByOidAsync(oid);
                return Ok(new ItemDto
                {
                    Oid = model.Oid,
                    Discount = model.Discount,
                    GSTAmount = model.GSTAmount,
                    GSTRate = model.GSTRate,
                    ItemDescription = model.ItemDescription,
                    ItemPrice = model.ItemPrice,
                    MaximumDiscount = model.MaximumDiscount,
                    AssessmentCode = model.AssessmentCode,
                    AssessmentDescription = model.AssessmentDescription,
                    AssessmentName = model.AssessmentName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=ItemController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
