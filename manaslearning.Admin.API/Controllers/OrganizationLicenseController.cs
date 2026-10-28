using manaslearning.Admin.API.Attributes;
using manaslearning.Admin.API.Models;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace manaslearning.Admin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationLicenseController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ILicenseRepository _licenseRepository;        

        public OrganizationLicenseController(
            ILogger<OrganizationLicenseController> logger,
            IOrganizationRepository organizationRepository,
            IAssessmentRepository assessmentRepository,
            IItemRepository itemRepository,
            ILicenseRepository licenseRepository            
            )
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
            _assessmentRepository = assessmentRepository;
            _itemRepository = itemRepository;
            _licenseRepository = licenseRepository;            
        }

        
        [HttpGet("GetByOrganizationOidAsync")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationLicense_Read)]
        public async Task<IActionResult> GetByOrganizationOidAsync(Guid organizationOid)
        {
            try
            {
                var OrganizationLicenses = await _licenseRepository.GetLicensesInfoByOrganizationOidAsync(organizationOid);
                if (OrganizationLicenses == null || !OrganizationLicenses.Any())
                    return NotFound();

                return Ok(OrganizationLicenses
                    .Select(l => new OrganizationLicenseDto
                    {
                        AssessmentId = l.AssessmentId,
                        AssessmentName = l.AssessmentName,
                        AssessmentOid = l.AssessmentOid,
                        AvailableLicenses = l.AvailableLicenses,
                        InitialPurchaseDate = l.InitialPurchaseDate,
                        LicenseId = l.LicenseId,
                        OrganizationId = l.OrganizationId,
                        RecentPurchanseDate = l.RecentPurchanseDate,
                        UsedLicenses = l.UsedLicenses
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationLicenseController.GetByOrganizationOidAsync(), OrganizaionOid:{organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }        


        // POST api/OrganizationLicense/Add        
        [HttpPost("Add")]
        [SuppressModelStateInvalidFilter]
        [AllowedPrivileges(PrivilegeTypes.OrganizationLicense_Add)]
        public async Task<IActionResult> AddAsync([FromBody] OrganizationLicenseAddModel model)
        {            
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationLicenseController.AddAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationByOidAsync(model.OrganizationOid);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationLicenseController.AddAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var assessmentInfo = await _assessmentRepository.GetAssessmentInfoByGuidIdAsync(organization.OrganizationId, model.AssessmentOid);
                if (assessmentInfo == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationLicenseController.AddAsync(), OrganizationOid: {model.OrganizationOid}, AssessmentOid: {model.AssessmentOid}");
                    return this.BadRequest();
                }

                var items = await _itemRepository.GetItemsInfoByAssessmentIdAsync(assessmentInfo.AssessmentId);
                if (items == null || !items.Any())
                {
                    _logger.LogError($"Items is null. Source:OrganizationLicenseController.AddAsync(), OrganizationOid: {model.OrganizationOid}, AssessmentOid: {model.AssessmentOid}");
                    return this.BadRequest();
                }

                if ((model.ItemOid != null && !items.Any(i => i.Oid == model.ItemOid)) || (model.ItemOid == null && items.Count() > 1))
                {
                    _logger.LogError($"Incorrect ItemOid or Items Count. Source:OrganizationLicenseController.AddAsync(), OrganizationOid: {model.OrganizationOid}, AssessmentOid: {model.AssessmentOid}, ItemOid: {model.ItemOid}");
                    return this.BadRequest();
                }

                int itemId = model.ItemOid != null ? items.First(i => i.Oid == model.ItemOid).ItemId : items.First().ItemId;

                int result = await _licenseRepository.AddOrganizationLicensesAsync
                                        (
                                            organization.OrganizationId,
                                            assessmentInfo.AssessmentId,
                                            itemId,
                                            model.Licenses,
                                            0 // Created By                                            
                                        );

                if (result <= 0)
                { throw new Exception($"Error occured while inserting the OrganizationLicense. AddOrganizationLicensesResult: {result}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationLicenseController.AddAsync(), OrganizationOid: {model.OrganizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/OrganizationLicense/Reduce        
        [HttpPost("Reduce")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationLicense_Reduce)]
        public async Task<IActionResult> ReduceAsync([FromBody] OrganizationLicenseReduceModel model)
        {
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationLicenseController.ReduceAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var organizationLicenses = await _licenseRepository.GetLicensesInfoByOrganizationOidAsync(model.OrganizationOid);
                if (organizationLicenses == null)
                {
                    _logger.LogError($"OrganizationLicenses is null. Source:OrganizationLicenseController.ReduceAsync(), OrganizationOid: {model.OrganizationOid}, AssessmentOid: {model.AssessmentOid}");
                    return this.BadRequest();
                }

                var organizationLicense = organizationLicenses.Where(ol => ol.AssessmentOid == model.AssessmentOid).FirstOrDefault();
                if (organizationLicense == null)
                {
                    _logger.LogError($"OrganizationLicense is null. Source:OrganizationLicenseController.ReduceAsync(), OrganizationOid: {model.OrganizationOid}, AssessmentOid: {model.AssessmentOid}");
                    return this.BadRequest();
                }

                if (organizationLicense.AvailableLicenses < model.Licenses)
                {
                    _logger.LogError($"Licenses should be less than Available licenses. Source:OrganizationLicenseController.ReduceAsync(), OrganizationOid: {model.OrganizationOid}, AssessmentOid: {model.AssessmentOid}");
                    return this.BadRequest();
                }


                int result = await _licenseRepository.ReduceOrganizationLicensesAsync(
                                            organizationLicense.OrganizationId,
                                            organizationLicense.AssessmentId,                                            
                                            model.Licenses,
                                            0 // Created By                                            
                                        );

                if (result < 0)
                { throw new Exception($"Error occured while updating the OrganizationLicense. ReduceOrganizationLicensesResult: {result}, OrganizationOid:{model.OrganizationOid}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationLicenseController.ReduceAsync(), OrganizationOid: {model.OrganizationOid}, AssessmentOid: {model.AssessmentOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }        
    }
}
