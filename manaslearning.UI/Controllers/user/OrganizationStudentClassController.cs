using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Attributes;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace manaslearning.UI.Controllers.user
{
    [Route("api/u/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpGet("All")]        
        public async Task<IActionResult> GetAllAsync()
        {
            int organizationId = 0;
            try
            {
                organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationStudentClassController.GetAll();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                var organizationStudentClasses = await _studentClassRepository.GetAllStudentClassesByOrganizationIdAsync(organizationId);
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
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationStudentClassController.GetAllAsync(), OrganizationId:{organizationId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]        
        public async Task<IActionResult> GetAsync()
        {
            int organizationId = 0;
            try
            {
                organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationStudentClassController.Get();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }
                var OrganizationStudentClasss = await _studentClassRepository.GetStudentClassesByOrganizationIdAsync(organizationId);
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
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationStudentClassController.GetAsync(), OrganizationId:{organizationId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/OrganizationStudentClass?StudentClassOid={StudentClassOid}
        [HttpGet("{StudentClassOid:Guid}")]        
        public async Task<IActionResult> GetAsync(Guid studentClassOid)
        {
            int organizationId = 0;
            try
            {
                organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationStudentClassController.Get();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }
                var model = await _studentClassRepository.GetStudentClassByOrganizationIdAsync(organizationId, studentClassOid);
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
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationStudentClassController.GetAsync(), OrganizatinId:{organizationId}, StudentClassOid:{studentClassOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/OrganizationStudentClass        
        [HttpPost]
        [SuppressModelStateInvalidFilter]       
        public async Task<IActionResult> PostAsync([FromForm] OrganizationStudentClassPostModel model)
        {
            int organizationId = 0;
            try
            {
                organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationStudentClassController.PostAsync();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                long userId = HttpContext.GetUserId();
                if (userId == 0)
                {
                    _logger.LogError($"Error=Incorrect UserId. Source=OrganizationStudentClassController.PostAsync();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationStudentClassController.PostAsync(), OrganizationId: {organizationId}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationAsync(organizationId);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationStudentClassController.PostAsync(), OrganizationOid: {organizationId}");
                    return this.BadRequest();
                }

                var classes = await _studentClassRepository.GetAllStudentClassesByOrganizationIdAsync(organization.OrganizationId);
                if (classes != null && classes.Any(c => c.ClassName.Trim() == model.StudentClassName.Trim()))
                {
                    return this.Conflict("Class is already exists"); 
                }

                int organizationStudentClassId = await _studentClassRepository.CreateOrganizationStudentClassAsync
                                        (
                                            organization.OrganizationId,
                                            model.StudentClassName?.Trim(),
                                            model.StudentClassDescription?.Trim(),
                                            userId                                         
                                        );

                if (organizationStudentClassId < 0)
                { throw new Exception($"Error occured while inserting the OrganizationStudentClass. OrganizationStudentClassId: {organizationStudentClassId}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationStudentClassController.PostAsync(), ClassName: {model.StudentClassName}, OrganizationId: {organizationId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/OrganizationStudentClass        
        [HttpPut]
        [SuppressModelStateInvalidFilter]
        public async Task<IActionResult> PutAsync([FromForm] OrganizationStudentClassPutModel model)
        {
            int organizationId = 0;
            try
            {
                organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationStudentClassController.PutAsync();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                long userId = HttpContext.GetUserId();
                if (userId == 0)
                {
                    _logger.LogError($"Error=Incorrect UserId. Source=OrganizationStudentClassController.PutAsync();OrganizationId={organizationId},UserId={userId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationStudentClassController.PutAsync(), OrganizationId: {organizationId}, StudentClassOid: {model.StudentClassOid}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationAsync(organizationId);
                if (organization == null)
                {
                    _logger.LogError($"Error=Organization is null. Source=OrganizationStudentClassController.PutAsync();OrganizationId={organizationId},UserId={userId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }               

                var organizationStudentClass = await _studentClassRepository.GetOrganizationStudentClassAsync(organization.Oid, model.StudentClassOid);
                if (organizationStudentClass == null)
                {
                    _logger.LogError($"OrganizationStudentClass is null. Source:OrganizationStudentClassController.PutAsync(), OrganizationId: {organizationId}, StudentClassId: {model.StudentClassOid}");
                    return this.BadRequest();
                }

                var classes = await _studentClassRepository.GetAllStudentClassesByOrganizationIdAsync(organization.OrganizationId);
                if (classes != null && classes.Where(c => c.Oid != model.StudentClassOid).Any(c => c.ClassName.Trim() == model.StudentClassName.Trim()))
                {
                    return this.Conflict("Class is already exists");
                }

                int organizationStudentClassId = await _studentClassRepository.UpdateOrganizationStudentClassAsync
                                        (
                                            organizationStudentClass.ClassId,
                                            organization.OrganizationId,
                                            model.StudentClassName?.Trim(),
                                            model.StudentClassDescription?.Trim(),
                                            model.IsActive,
                                            userId                                          
                                        );

                if (organizationStudentClassId < 0)
                { throw new Exception($"Error occured while updating the OrganizationStudentClass. OrganizationStudentClassId: {organizationStudentClassId}, OrganizationId:{organizationId}"); }

                return Ok(organizationStudentClass);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationStudentClassController.PutAsync(), StudentClassId: {model.StudentClassOid}, OrganizationId: {organizationId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // DELETE api/OrganizationStudentClass/studentClassOid        
        [HttpDelete]        
        public async Task<IActionResult> DeleteAsync(Guid studentClassOid)
        {
            int organizationId = 0;
            try
            {
                organizationId = HttpContext.GetOrganizationId();
                if (organizationId == 0 || HttpContext.GetLoginType() != Enums.LoginTypes.OrganiztionAdmin)
                {
                    _logger.LogError($"Error=Incorrect OrganizationId or LoginType. Source=OrganizationStudentClassController.DeleteAsync();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                var organization = await _organizationRepository.GetOrganizationAsync(organizationId);
                if (organization == null)
                {
                    _logger.LogError($"Error=Organization is null. Source=OrganizationStudentClassController.DeleteAsync();OrganizationId={organizationId},LoginType={HttpContext.GetLoginType()}");
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                }

                var organizationStudentClass = await _studentClassRepository.GetOrganizationStudentClassAsync(organization.Oid, studentClassOid);
                if (organizationStudentClass == null)
                {
                    _logger.LogError($"Organization Student Class is null. Source:OrganizationStudentClassController.DeleteAsync(), OrganizationId: {organizationId}, StudentClassOid: {studentClassOid}");
                    return this.BadRequest();
                }

                int deleteOrganizationStudentClassResult = await _studentClassRepository.DeleteOrganizationStudentClassAsync(organizationStudentClass.ClassId);
                if (deleteOrganizationStudentClassResult != 1)
                { throw new Exception($"Error occured while deleting the OrganizationStudentClass, Result: {deleteOrganizationStudentClassResult}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationStudentClassController.DeleteAsync(), StudentOid: {studentClassOid}, OrganizationId: {organizationId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
