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
    public class OrganizationAddressController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationAddressRepository _organizationAddressRepository;        

        public OrganizationAddressController(
            ILogger<OrganizationAddressController> logger,
            IOrganizationRepository organizationRepository,
            IOrganizationAddressRepository organizationAddressRepository            
            )
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
            _organizationAddressRepository = organizationAddressRepository;            
        }

        [HttpGet("GetAllByOrganizationOidAsync")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationAddress_ReadAll)]
        public async Task<IActionResult> GetAllByOrganizationOidAsync(Guid organizationOid)
        {
            try
            {
                var organizationAddresses = await _organizationAddressRepository.GetAllOrganizationAddressesByOrganizationOidAsync(organizationOid);
                if (organizationAddresses == null || !organizationAddresses.Any())
                    return NotFound();

                return Ok(organizationAddresses
                    .OrderByDescending(x => x.AddressId)
                    .Select(a => new OrganizationAddressDto
                    {
                        AddressId = a.AddressId,
                        AddressTypeId = a.AddressTypeId,
                        AddressLine1 = a.AddressLine1,
                        AddressLine2 = a.AddressLine2,
                        AddressLine3 = a.AddressLine3,
                        City = a.City,
                        State = a.State,
                        Country = a.Country,
                        IsDefault = a.IsDefault,
                        Oid = a.Oid,
                        OrganizationId = a.OrganizationId,
                        Phone1 = a.Phone1,
                        Phone2 = a.Phone2,
                        ZipCode = a.ZipCode                        
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationAddressController.GetAllByOrganizationOidAsync(), OrganizaionOid:{organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("GetByOrganizationOidAsync")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationAddress_Read)]
        public async Task<IActionResult> GetByOrganizationOidAsync(Guid organizationOid)
        {
            try
            {
                var OrganizationAddresss = await _organizationAddressRepository.GetOrganizationAddressesByOrganizationOidAsync(organizationOid);
                if (OrganizationAddresss == null || !OrganizationAddresss.Any())
                    return NotFound();

                return Ok(OrganizationAddresss
                    .OrderByDescending(x => x.AddressId)                    
                    .Select(a => new OrganizationAddressDto
                    {
                        AddressId = a.AddressId,
                        AddressTypeId = a.AddressTypeId,
                        AddressLine1 = a.AddressLine1,
                        AddressLine2 = a.AddressLine2,
                        AddressLine3 = a.AddressLine3,
                        City = a.City,
                        State = a.State,
                        Country = a.Country,
                        IsDefault = a.IsDefault,
                        Oid = a.Oid,
                        OrganizationId = a.OrganizationId,
                        Phone1 = a.Phone1,
                        Phone2 = a.Phone2,
                        ZipCode = a.ZipCode
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationAddressController.GetByOrganizationOidAsync(), OrganizaionOid:{organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/OrganizationAddress?OrganizationOid={OrganizationOid}&OrganizationAddressOid={OrganizationAddressOid}
        [HttpGet("{OrganizationOid:Guid}/{OrganizationAddressOid:Guid}")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationAddress_Read)]
        public async Task<IActionResult> GetAsync(Guid organizationOid, Guid organizationAddressOid)
        {
            try
            {
                var model = await _organizationAddressRepository.GetOrganizationAddressAsync(organizationOid, organizationAddressOid);
                if (model == null)
                { throw new Exception($"Error occured while fetching the OrganizationAddress."); }               

                return Ok(new OrganizationAddressDto
                {
                    AddressId = model.AddressId,
                    AddressTypeId = model.AddressTypeId,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    AddressLine3 = model.AddressLine3,
                    City = model.City,
                    State = model.State,
                    Country = model.Country,
                    IsDefault = model.IsDefault,
                    Oid = model.Oid,
                    OrganizationId = model.OrganizationId,
                    Phone1 = model.Phone1,
                    Phone2 = model.Phone2
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationAddressController.GetAsync(), OrganizatinOid:{organizationOid}, OrganizationAddressOid:{organizationAddressOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        // POST api/OrganizationAddress        
        [HttpPost]
        [SuppressModelStateInvalidFilter]
        [AllowedPrivileges(PrivilegeTypes.OrganizationAddress_Create)]
        public async Task<IActionResult> PostAsync([FromBody] OrganizationAddressPostModel model)
        {            
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationAddressController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationByOidAsync(model.OrganizationOid);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationAddressController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                int organizationAddressId = await _organizationAddressRepository.CreateOrganizationAddressAsync
                                        (
                                            organization.OrganizationId,
                                            model.AddressTypeId,
                                            model.AddressLine1?.Trim(),
                                            model.AddressLine2?.Trim(),
                                            model.AddressLine3?.Trim(),
                                            model.City?.Trim(),
                                            model.State?.Trim(),
                                            model.Country?.Trim(),
                                            model.ZipCode?.Trim(),
                                            model.Phone1?.Trim(),
                                            model.Phone2?.Trim(),
                                            model.IsDefault
                                        );

                if (organizationAddressId < 0)
                { throw new Exception($"Error occured while inserting the OrganizationAddress. OrganizationAddressId: {organizationAddressId}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationAddressController.PostAsync(), AddressLine1: {model.AddressLine1}, OrganizationOid: {model.OrganizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/OrganizationAddress        
        [HttpPut]
        [AllowedPrivileges(PrivilegeTypes.OrganizationAddress_Update)]
        public async Task<IActionResult> PutAsync([FromBody] OrganizationAddressPutModel model)
        {
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationAddressController.PutAsync(), AddressOid: {model.AddressOid}");
                    return this.BadRequest();
                }

                var organizationAddress = await _organizationAddressRepository.GetOrganizationAddressAsync(model.OrganizationOid, model.AddressOid);
                if (organizationAddress == null)
                {
                    _logger.LogError($"OrganizationAddress is null. Source:OrganizationAddressController.PutAsync(), OrganizationOid: {model.OrganizationOid}, AddressOid: {model.AddressOid}");
                    return this.BadRequest();
                }

                int organizationAddressId = await _organizationAddressRepository.UpdateOrganizationAddressAsync
                                        (
                                            organizationAddress.AddressId,
                                            organizationAddress.OrganizationId,
                                            model.AddressTypeId,
                                            model.AddressLine1?.Trim(),
                                            model.AddressLine2?.Trim(),
                                            model.AddressLine3?.Trim(),
                                            model.City?.Trim(),
                                            model.State?.Trim(),
                                            model.Country?.Trim(),
                                            model.ZipCode?.Trim(),
                                            model.Phone1?.Trim(),
                                            model.Phone2?.Trim(),
                                            model.IsDefault
                                        );

                if (organizationAddressId < 0)
                { throw new Exception($"Error occured while updating the OrganizationAddress. OrganizationAddressId: {organizationAddressId}, OrganizationOid:{model.OrganizationOid}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationAddressController.PutAsync(), AddressOid: {model.AddressOid}, OrganizationOid: {model.OrganizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // DELETE api/OrganizationAddress/oid        
        [HttpDelete]
        [AllowedPrivileges(PrivilegeTypes.OrganizationAddress_Delete)]
        public async Task<IActionResult> DeleteAsync(Guid organizationOid, Guid addressOid)
        {
            try
            {
                var organizationAddress = await _organizationAddressRepository.GetOrganizationAddressAsync(organizationOid, addressOid);
                if (organizationAddress == null)
                {
                    _logger.LogError($"OrganizationAddress is null. Source:OrganizationAddressController.DeleteAsync(), OrganizationOid: {organizationOid}, AddressOid: {addressOid}");
                    return this.BadRequest();
                }

                int result = await _organizationAddressRepository.DeleteOrganizationAddressAsync(addressOid);
                if (result != 1)
                { throw new Exception($"Error occured while deleting the OrganizationAddress, AddressId : {result}"); }
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationAddressController.DeleteAsync(), AddressOid: {addressOid}, OrganizationOid: {organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
