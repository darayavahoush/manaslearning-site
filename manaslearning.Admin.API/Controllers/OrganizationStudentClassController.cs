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
    public class OrganizationStudentClassController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        
           
        public OrganizationStudentClassController(
            ILogger<OrganizationStudentClassController> logger,
            IOrganizationRepository organizationRepository,
            IStudentClassRepository studentClassRepository
            )
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
            _studentClassRepository = studentClassRepository;
        }

        [HttpGet("GetAllByOrganizationOidAsync")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudentClass_ReadAll)]
        public async Task<IActionResult> GetAllByOrganizationOidAsync(Guid organizationOid)
        {
            try
            {
                var organizationStudentClasses = await _studentClassRepository.GetAllStudentClassesByOrganizationOidAsync(organizationOid);
                if (organizationStudentClasses == null || !organizationStudentClasses.Any())
                    return Ok(new List<OrganizationStudentClassDto>());

                return Ok(organizationStudentClasses                    
                    .Select(sc => new OrganizationStudentClassDto
                    {
                       StudentClassDescription = sc.ClassDescription,
                       StudentClassName = sc.ClassName,
                       StudentClassOid = sc.Oid,
                       IsActive = sc.IsActive
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationStudentClassController.GetAllByOrganizationOidAsync(), OrganizaionOid:{organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("GetByOrganizationOidAsync")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudentClass_Read)]
        public async Task<IActionResult> GetByOrganizationOidAsync(Guid organizationOid)
        {
            try
            {
                var OrganizationStudentClasss = await _studentClassRepository.GetStudentClassesByOrganizationOidAsync(organizationOid);
                if (OrganizationStudentClasss == null || !OrganizationStudentClasss.Any())
                    return Ok(new List<OrganizationStudentClassDto>());

                return Ok(OrganizationStudentClasss
                    .Select(sc => new OrganizationStudentClassDto
                    {
                        StudentClassDescription = sc.ClassDescription,
                        StudentClassName = sc.ClassName,
                        StudentClassOid = sc.Oid,
                        IsActive = sc.IsActive
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationStudentClassController.GetByOrganizationOidAsync(), OrganizaionOid:{organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/OrganizationStudentClass?OrganizationOid={OrganizationOid}&StudentClassOid={StudentClassOid}
        [HttpGet("{OrganizationOid:Guid}/{StudentClassOid:Guid}")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudentClass_Read)]
        public async Task<IActionResult> GetAsync(Guid organizationOid, Guid studentClassOid)
        {
            try
            {
                var model = await _studentClassRepository.GetOrganizationStudentClassAsync(organizationOid, studentClassOid);
                if (model == null)
                { return NotFound(StringConstants.RECORD_NOTFOUND); }               

                return Ok(new OrganizationStudentClassDto
                {
                    StudentClassOid = model.Oid,
                    StudentClassName = model.ClassName,
                    StudentClassDescription = model.ClassDescription,
                    IsActive = model.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationStudentClassController.GetAsync(), OrganizatinOid:{organizationOid}, StudentClassOid:{studentClassOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/OrganizationStudentClass        
        [HttpPost]
        [SuppressModelStateInvalidFilter]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudentClass_Create)]
        public async Task<IActionResult> PostAsync([FromBody] OrganizationStudentClassPostModel model)
        {            
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationStudentClassController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationByOidAsync(model.OrganizationOid);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationStudentClassController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }               

                int organizationStudentClassId = await _studentClassRepository.CreateOrganizationStudentClassAsync
                                        (
                                            organization.OrganizationId,
                                            model.StudentClassName,
                                            model.StudentClassDescription,
                                            0 // UserId                                            
                                        );

                if (organizationStudentClassId < 0)
                { throw new Exception($"Error occured while inserting the OrganizationStudentClass. OrganizationStudentClassId: {organizationStudentClassId}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationStudentClassController.PostAsync(), ClassName: {model.StudentClassName}, OrganizationOid: {model.OrganizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/OrganizationStudentClass        
        [HttpPut]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudentClass_Update)]
        public async Task<IActionResult> PutAsync([FromBody] OrganizationStudentClassPutModel model)
        {
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationStudentClassController.PutAsync(), OrganizationOid: {model.OrganizationOid}, StudentClassOid: {model.StudentClassOid}");
                    return this.BadRequest();
                }

                var organizationStudentClass = await _studentClassRepository.GetOrganizationStudentClassAsync(model.OrganizationOid, model.StudentClassOid);
                if (organizationStudentClass == null)
                {
                    _logger.LogError($"OrganizationStudentClass is null. Source:OrganizationStudentClassController.PutAsync(), OrganizationOid: {model.OrganizationOid}, StudentClassId: {model.StudentClassOid}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationByOidAsync(model.OrganizationOid);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationStudentClassController.PutAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                int organizationStudentClassId = await _studentClassRepository.UpdateOrganizationStudentClassAsync
                                        (
                                            organizationStudentClass.ClassId, 
                                            organization.OrganizationId,
                                            model.StudentClassName,
                                            model.StudentClassDescription,
                                            model.IsActive,
                                            0 //UserId                                            
                                        );

                if (organizationStudentClassId < 0)
                { throw new Exception($"Error occured while updating the OrganizationStudentClass. OrganizationStudentClassId: {organizationStudentClassId}, OrganizationOid:{model.OrganizationOid}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationStudentClassController.PutAsync(), StudentClassId: {model.StudentClassOid}, OrganizationOid: {model.OrganizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // DELETE api/OrganizationStudentClass/organizationOid/studentClassOid        
        [HttpDelete]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudentClass_Delete)]
        public async Task<IActionResult> DeleteAsync(Guid organizationOid, Guid studentClassOid)
        {
            try
            {
                var organizationStudentClass = await _studentClassRepository.GetOrganizationStudentClassAsync(organizationOid, studentClassOid);
                if (organizationStudentClass == null)
                {
                    _logger.LogError($"Organization Student Class is null. Source:OrganizationStudentClassController.DeleteAsync(), OrganizationOid: {organizationOid}, StudentClassOid: {studentClassOid}");
                    return this.BadRequest();
                }

                int deleteOrganizationStudentClassResult = await _studentClassRepository.DeleteOrganizationStudentClassAsync(organizationStudentClass.ClassId);
                if (deleteOrganizationStudentClassResult != 1)
                { throw new Exception($"Error occured while deleting the OrganizationStudentClass, Result: {deleteOrganizationStudentClassResult}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationStudentClassController.DeleteAsync(), StudentOid: {studentClassOid}, OrganizationOid: {organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
