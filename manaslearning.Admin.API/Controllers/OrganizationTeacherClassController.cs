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
    public class OrganizationTeacherClassController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ITeacherClassRepository _teacherClassRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        
           
        public OrganizationTeacherClassController(
            ILogger<OrganizationTeacherClassController> logger,
            IOrganizationRepository organizationRepository,
            ITeacherClassRepository teacherClassRepository,
            IStudentClassRepository studentClassRepository
            )
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
            _teacherClassRepository = teacherClassRepository;
            _studentClassRepository = studentClassRepository;
        }

        [HttpGet("GetAllByOrganizationOidAsync")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationTeacherClass_ReadAll)]
        public async Task<IActionResult> GetAllByOrganizationOidAsync(Guid organizationOid)
        {
            try
            {
                var organizationTeacherClasses = await _teacherClassRepository.GetAllTeacherClassesInfoByOrganizationOidAsync(organizationOid);
                if (organizationTeacherClasses == null || !organizationTeacherClasses.Any())
                    return Ok(new List<OrganizationTeacherClassDto>());

                return Ok(organizationTeacherClasses                    
                    .Select(tc => new OrganizationTeacherClassDto
                    {
                       StudentClassDescription = tc.ClassDescription,
                       StudentClassName = tc.ClassName,
                       StudentClassOid = tc.ClassOid,
                       TeacherId = tc.TeacherId,
                       TeacherName = tc.TeacherName,
                       TeacherClassOid = tc.Oid,
                       IsActive = tc.IsActive
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationTeacherClassController.GetAllByOrganizationOidAsync(), OrganizaionOid:{organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("GetByOrganizationOidAsync")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationTeacherClass_Read)]
        public async Task<IActionResult> GetByOrganizationOidAsync(Guid organizationOid)
        {
            try
            {
                var OrganizationTeacherClasss = await _teacherClassRepository.GetTeacherClassesInfoByOrganizationOidAsync(organizationOid);
                if (OrganizationTeacherClasss == null || !OrganizationTeacherClasss.Any())
                    return Ok(new List<OrganizationTeacherClassDto>());

                return Ok(OrganizationTeacherClasss
                    .Select(tc => new OrganizationTeacherClassDto
                    {
                        StudentClassDescription = tc.ClassDescription,
                        StudentClassName = tc.ClassName,
                        StudentClassOid = tc.ClassOid,
                        TeacherId = tc.TeacherId,
                        TeacherName = tc.TeacherName,
                        TeacherClassOid = tc.Oid,
                        IsActive = tc.IsActive
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationTeacherClassController.GetByOrganizationOidAsync(), OrganizaionOid:{organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/OrganizationTeacherClass?OrganizationOid={OrganizationOid}&TeacherClassOid={TeacherClassOid}
        [HttpGet("{OrganizationOid:Guid}/{TeacherClassOid:Guid}")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationTeacherClass_Read)]
        public async Task<IActionResult> GetAsync(Guid organizationOid, Guid teacherClassOid)
        {
            try
            {
                var model = await _teacherClassRepository.GetOrganizationTeacherClassInfoAsync(organizationOid, teacherClassOid);
                if (model == null)
                { return NotFound(StringConstants.RECORD_NOTFOUND); }               

                return Ok(new OrganizationTeacherClassDto
                {
                    StudentClassDescription = model.ClassDescription,
                    StudentClassName = model.ClassName,
                    StudentClassOid = model.ClassOid,
                    TeacherId = model.TeacherId,
                    TeacherName = model.TeacherName,
                    TeacherClassOid = model.Oid,
                    IsActive = model.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationTeacherClassController.GetAsync(), OrganizatinOid:{organizationOid}, TeacherClassOid:{teacherClassOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/OrganizationTeacherClass        
        [HttpPost]        
        [AllowedPrivileges(PrivilegeTypes.OrganizationTeacherClass_Create)]
        public async Task<IActionResult> PostAsync([FromBody] OrganizationTeacherClassPostModel model)
        {            
            try
            {
                if (model.TeacherId <= 0)
                {
                    _logger.LogError($"TeacherId is Invalid. Source:OrganizationTeacherClassController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationByOidAsync(model.OrganizationOid);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationTeacherClassController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var studentClass = await _studentClassRepository.GetOrganizationStudentClassAsync(model.OrganizationOid, model.StudentClassOid);
                if (organization == null)
                {
                    _logger.LogError($"StudentClass is null. Source:OrganizationTeacherClassController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                int organizationTeacherClassId = await _teacherClassRepository.CreateOrganizationTeacherClassAsync
                                        (
                                            organization.OrganizationId,
                                            model.TeacherId,
                                            studentClass.ClassId,
                                            0 // UserId                                            
                                        );

                if (organizationTeacherClassId < 0)
                { throw new Exception($"Error occured while inserting the OrganizationTeacherClass. OrganizationTeacherClassId: {organizationTeacherClassId}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationTeacherClassController.PostAsync(), ClassOid: {model.StudentClassOid}, OrganizationOid: {model.OrganizationOid}, Teacherid: {model.TeacherId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/OrganizationTeacherClass        
        [HttpPut]
        [AllowedPrivileges(PrivilegeTypes.OrganizationTeacherClass_Update)]
        public async Task<IActionResult> PutAsync([FromBody] OrganizationTeacherClassPutModel model)
        {
            try
            {
                if (model.TeacherId <= 0)
                {
                    _logger.LogError($"TeacherId is Invalid. Source:OrganizationTeacherClassController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationByOidAsync(model.OrganizationOid);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationTeacherClassController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var studentClass = await _studentClassRepository.GetOrganizationStudentClassAsync(model.OrganizationOid, model.StudentClassOid);
                if (organization == null)
                {
                    _logger.LogError($"StudentClass is null. Source:OrganizationTeacherClassController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var organizationTeacherClass = await _teacherClassRepository.GetOrganizationTeacherClassAsync(model.OrganizationOid, model.TeacherClassOid);
                if (organizationTeacherClass == null)
                {
                    _logger.LogError($"OrganizationTeacherClass is null. Source:OrganizationTeacherClassController.PutAsync(), OrganizationOid: {model.OrganizationOid}, TeacherClassId: {model.TeacherClassOid}");
                    return this.BadRequest();
                }
                
                int organizationTeacherClassUpdateResult = await _teacherClassRepository.UpdateOrganizationTeacherClassAsync
                                        (
                                            model.TeacherClassOid,
                                            model.TeacherId,
                                            studentClass.ClassId,
                                            organization.OrganizationId,
                                            model.IsActive,
                                            0 //UserId                                            
                                        );

                if (organizationTeacherClassUpdateResult < 0)
                { throw new Exception($"Error occured while updating the OrganizationTeacherClass. OrganizationTeacherClassId: {organizationTeacherClassUpdateResult}, OrganizationOid:{model.OrganizationOid}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationTeacherClassController.PutAsync(), TeacherClassId: {model.TeacherClassOid}, OrganizationOid: {model.OrganizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // DELETE api/OrganizationTeacherClass/organizationOid/teacherClassOid        
        [HttpDelete]
        [AllowedPrivileges(PrivilegeTypes.OrganizationTeacherClass_Delete)]
        public async Task<IActionResult> DeleteAsync(Guid organizationOid, Guid teacherClassOid)
        {
            try
            {
                var organizationTeacherClass = await _teacherClassRepository.GetOrganizationTeacherClassAsync(organizationOid, teacherClassOid);
                if (organizationTeacherClass == null)
                {
                    _logger.LogError($"Organization Teacher Class is null. Source:OrganizationTeacherClassController.DeleteAsync(), OrganizationOid: {organizationOid}, TeacherClassOid: {teacherClassOid}");
                    return this.BadRequest();
                }

                int deleteOrganizationTeacherClassResult = await _teacherClassRepository.DeleteOrganizationTeacherClassAsync(organizationTeacherClass.Oid);
                if (deleteOrganizationTeacherClassResult != 1)
                { throw new Exception($"Error occured while deleting the OrganizationTeacherClass, Result: {deleteOrganizationTeacherClassResult}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationTeacherClassController.DeleteAsync(), StudentOid: {teacherClassOid}, OrganizationOid: {organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
