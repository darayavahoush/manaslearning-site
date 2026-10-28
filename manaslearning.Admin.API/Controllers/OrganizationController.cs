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
    public class OrganizationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IProfileRepository _profileRepository;

        public OrganizationController(
            ILogger<OrganizationController> logger,
            IOrganizationRepository organizationRepository,
            IProfileRepository profileRepository
            )
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
            _profileRepository = profileRepository;
        }

        [HttpGet("All")]
        [AllowedPrivileges(PrivilegeTypes.Organization_ReadAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var organizations = await _organizationRepository.GetAllOrganizationsAsync();
                if (organizations == null || !organizations.Any())
                    return NotFound();

                return Ok(organizations
                    .OrderByDescending(x => x.OrganizationId)
                    .Select(o => new OrganizationDto
                    {
                        OrganizationId = o.OrganizationId,
                        Name = o.Name,
                        CreatedBy = o.CreatedBy,
                        CreatedDate = o.CreatedDate,
                        DisplayName = o.DisplayName,
                        Oid = o.Oid,
                        ModifiedBy = o.ModifiedBy,
                        ModifiedDate = o.ModifiedDate,
                        ParentOrganizationId = o.ParentOrganizationId,
                        OrganizationType = o.OrganizationTypeId,
                        IsActive = o.IsActive
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [AllowedPrivileges(PrivilegeTypes.Organization_Read)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var organizations = await _organizationRepository.GetOrganizationsAsync();
                if (organizations == null || !organizations.Any())
                    return Ok(new List<OrganizationDto>());

                return Ok(organizations
                    .OrderByDescending(x => x.OrganizationId)
                    .Select(o => new OrganizationDto
                    {
                        OrganizationId = o.OrganizationId,
                        Name = o.Name,
                        CreatedBy = o.CreatedBy,
                        CreatedDate = o.CreatedDate,
                        DisplayName = o.DisplayName,
                        Oid = o.Oid,
                        ModifiedBy = o.ModifiedBy,
                        ModifiedDate = o.ModifiedDate,
                        ParentOrganizationId = o.ParentOrganizationId,
                        OrganizationType = o.OrganizationTypeId,
                        IsActive = o.IsActive
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/Organization?OrganizationId={OrganizationId}
        [HttpGet("{OrganizationId:int}")]
        [AllowedPrivileges(PrivilegeTypes.Organization_Read)]
        public async Task<IActionResult> GetAsync(int organizationId)
        {
            try
            {
                var model = await _organizationRepository.GetOrganizationAsync(organizationId);
                if (model == null)
                    return NotFound();

                return Ok(new OrganizationDto
                {
                    OrganizationId = model.OrganizationId,
                    Name = model.Name,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = model.CreatedDate,
                    DisplayName = model.DisplayName,
                    Oid = model.Oid,
                    ModifiedBy = model.ModifiedBy,
                    ModifiedDate = model.ModifiedDate,
                    ParentOrganizationId = model.ParentOrganizationId,
                    OrganizationType = model.OrganizationTypeId,
                    IsActive = model.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("GetByOid/{Oid:Guid}")]
        [AllowedPrivileges(PrivilegeTypes.Organization_Read)]
        public async Task<IActionResult> GetByOidAsync(Guid oid)
        {
            try
            {
                var model = await _organizationRepository.GetOrganizationByOidAsync(oid);
                if (model == null)
                    return NotFound();

                return Ok(new OrganizationDto
                {
                    OrganizationId = model.OrganizationId,
                    Name = model.Name,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = model.CreatedDate,
                    DisplayName = model.DisplayName,
                    Oid = model.Oid,
                    ModifiedBy = model.ModifiedBy,
                    ModifiedDate = model.ModifiedDate,
                    ParentOrganizationId = model.ParentOrganizationId,
                    OrganizationType = model.OrganizationTypeId,
                    IsActive = model.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationController.GetByOidAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/Organization        
        [HttpPost]
        [SuppressModelStateInvalidFilter]
        [AllowedPrivileges(PrivilegeTypes.Organization_Create)]
        public async Task<IActionResult> PostAsync([FromBody] OrganizationPostModel model)
        {
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationController.PostAsync(), Name: {model.Name}");
                    return this.BadRequest();
                }

                int OrganizationId = await _organizationRepository.CreateOrganizationAsync(userId: 0, model.Name, model.DisplayName, model.ParentOrganizationId, model.OrganizationTypeId);

                if (OrganizationId == -2)
                    return Conflict("Organization already exists.");
                else if (OrganizationId == -3)
                    return BadRequest();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationController.PostAsync(), Name: {model.Name}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/Organization        
        [HttpPut]
        [AllowedPrivileges(PrivilegeTypes.Organization_Update)]
        public async Task<IActionResult> PutAsync([FromBody] OrganizationPutModel model)
        {
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationController.PutAsync(), Oid: {model.Oid}");
                    return this.BadRequest();
                }

                int OrganizationId = await _organizationRepository.UpdateOrganizationAsync
                                        (
                                            0, // UserId
                                            model.Name,
                                            model.DisplayName,
                                            model.OrganizationTypeId,
                                            model.ParentOrganizationId,
                                            model.DefaultAdmin,
                                            model.Oid
                                        );

                if (OrganizationId < 0)
                { throw new Exception($"Error occured while updating the Organization. OrganizationId: {OrganizationId}, Oid:{model.Oid}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationController.PutAsync(), Oid: {model.Oid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // DELETE api/Organization/oid        
        [HttpDelete]
        [AllowedPrivileges(PrivilegeTypes.Organization_Delete)]
        public async Task<IActionResult> DeleteAsync(Guid oid)
        {
            try
            {
                int result = await _organizationRepository.DeleteOrganizationAsync(oid, 0);
                if (result != 1)
                { throw new Exception($"Error occured while deleting the Organization, Oid : {oid}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationController.DeleteAsync(), Oid: {oid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("SetDefaultAdmin")]
        [SuppressModelStateInvalidFilter]
        [AllowedPrivileges(PrivilegeTypes.Organization_SetDefaultAdmin)]
        public async Task<IActionResult> SetDefaultAdminAsync([FromBody] OrganizationDefaultAdminModel model)
        {
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationController.SetDefaultAdminAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationByOidAsync(model.OrganizationOid);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationController.SetDefaultAdminAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var profile = await _profileRepository.GetUserInfoAsync(model.OrganizationOid, model.UserId);
                if (profile == null)
                {
                    _logger.LogError($"Profile is null. Source:OrganizationController.SetDefaultAdminAsync(), UserId: {model.UserId}, OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                int result = await _organizationRepository.SetOranizationDefaultAdminAsync(organization.OrganizationId, model.UserId, 0);

                if (result <= 0)
                { throw new Exception($"Error occured while setting the default admin. SetOrganizationDefaultAdminResult: {result}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationController.SetDefaultAdminAsync(), OrganizationOid: {model.OrganizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
