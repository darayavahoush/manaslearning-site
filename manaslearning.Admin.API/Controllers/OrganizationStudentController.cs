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
    public class OrganizationStudentController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly IQualificationRepository _qualificationRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IGenderRepository _genderRepository;
        private readonly ISchoolTypeRepository _schoolTypeRepository;
        private readonly ISyllabusTypeRepository _syllabusTypeRepository;
           
        public OrganizationStudentController(
            ILogger<OrganizationStudentController> logger,
            IOrganizationRepository organizationRepository,
            IStudentRepository studentRepository,
            IStudentClassRepository studentClassRepository,
            IQualificationRepository qualificationRepository,
            ILanguageRepository languageRepository,
            IGenderRepository genderRepository,
            ISchoolTypeRepository schoolTypeRepository,
            ISyllabusTypeRepository syllabusTypeRepository            
            )
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
            _studentRepository = studentRepository;
            _studentClassRepository = studentClassRepository;
            _qualificationRepository = qualificationRepository;
            _languageRepository = languageRepository;
            _genderRepository = genderRepository;
            _schoolTypeRepository = schoolTypeRepository;
            _syllabusTypeRepository = syllabusTypeRepository;
        }

        [HttpGet("GetAllByOrganizationOidAsync")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudent_ReadAll)]
        public async Task<IActionResult> GetAllByOrganizationOidAsync(Guid organizationOid)
        {
            try
            {
                var organizationStudents = await _studentRepository.GetAllOrganizationStudentsByOrganizationOidAsync(organizationOid);
                if (organizationStudents == null || !organizationStudents.Any())
                    return Ok(new List<OrganizationStudentDto>());

                return Ok(organizationStudents                    
                    .Select(os => new OrganizationStudentDto
                    {
                       DateOfBirth = os.DateOfBirth,
                       Email = os.Email,
                       Firstlanguage = os.Firstlanguage,
                       Gender = os.Gender,
                       LanguagesCanSpeak = os.LanguagesCanSpeak,
                       MotherTongue = os.MotherTongue,
                       ParentEducation = os.ParentEducation,
                       ParentName = os.ParentName,
                       PhoneNumber = os.PhoneNumber,
                       SchoolType = os.SchoolType,
                       SecondLanguage = os.SecondLanguage,
                       StudentClass = os.StudentClass,
                       StudentClassName = os.StudentClassName,
                       StudentOid = os.Oid,
                       StudentName = os.StudentName,
                       SyllabusType = os.SyllabusType,
                       ThirdLanguage = os.ThirdLanguage,
                       Zip = os.Zip,
                       IsActive = os.IsActive
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationStudentController.GetAllByOrganizationOidAsync(), OrganizaionOid:{organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("GetByOrganizationOidAsync")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudent_Read)]
        public async Task<IActionResult> GetByOrganizationOidAsync(Guid organizationOid)
        {
            try
            {
                var organizationStudents = await _studentRepository.GetOrganizationStudentsByOrganizationOidAsync(organizationOid);
                if (organizationStudents == null || !organizationStudents.Any())
                    return Ok(new List<OrganizationStudentDto>());

                return Ok(organizationStudents
                    .Select(os => new OrganizationStudentDto
                    {
                        DateOfBirth = os.DateOfBirth,
                        Email = os.Email,
                        Firstlanguage = os.Firstlanguage,
                        Gender = os.Gender,
                        LanguagesCanSpeak = os.LanguagesCanSpeak,
                        MotherTongue = os.MotherTongue,
                        ParentEducation = os.ParentEducation,
                        ParentName = os.ParentName,
                        PhoneNumber = os.PhoneNumber,
                        SchoolType = os.SchoolType,
                        SecondLanguage = os.SecondLanguage,
                        StudentClass = os.StudentClass,
                        StudentClassName = os.StudentClassName,
                        StudentOid = os.Oid,
                        StudentName = os.StudentName,
                        SyllabusType = os.SyllabusType,
                        ThirdLanguage = os.ThirdLanguage,
                        Zip = os.Zip,
                        IsActive = os.IsActive
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationStudentController.GetByOrganizationOidAsync(), OrganizaionOid:{organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/OrganizationStudent?OrganizationOid={OrganizationOid}&StudentOid={StudentOid}
        [HttpGet("{OrganizationOid:Guid}/{StudentOid:Guid}")]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudent_Read)]
        public async Task<IActionResult> GetAsync(Guid organizationOid, Guid studentOid)
        {
            try
            {
                var model = await _studentRepository.GetOrganizationStudentAsync(organizationOid, studentOid);
                if (model == null)
                { return NotFound(StringConstants.RECORD_NOTFOUND); }               

                return Ok(new OrganizationStudentDto
                {
                    DateOfBirth = model.DateOfBirth,
                    Email = model.Email,
                    Firstlanguage = model.Firstlanguage,
                    Gender = model.Gender,
                    LanguagesCanSpeak = model.LanguagesCanSpeak,
                    MotherTongue = model.MotherTongue,
                    ParentEducation = model.ParentEducation,
                    ParentName = model.ParentName,
                    PhoneNumber = model.PhoneNumber,
                    SchoolType = model.SchoolType,
                    SecondLanguage = model.SecondLanguage,
                    StudentClass = model.StudentClass,
                    StudentClassName = model.StudentClassName,
                    StudentOid = model.Oid,
                    StudentName = model.StudentName,
                    SyllabusType = model.SyllabusType,
                    ThirdLanguage = model.ThirdLanguage,
                    Zip = model.Zip,
                    IsActive = model.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrganizationStudentController.GetAsync(), OrganizatinOid:{organizationOid}, StudentOid:{studentOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/OrganizationStudent        
        [HttpPost]
        [SuppressModelStateInvalidFilter]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudent_Create)]
        public async Task<IActionResult> PostAsync([FromBody] OrganizationStudentPostModel model)
        {            
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationByOidAsync(model.OrganizationOid);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }                

                var studentClasses = _studentClassRepository.GetStudentClassesAsync().Result;
                if (!studentClasses.Any(c => c.ClassId == model.StudentClass))
                {
                    _logger.LogError($"Incorrect StudentClass. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, StudentClass: {model.StudentClass}");
                    return this.BadRequest();
                }

                var qualifications = _qualificationRepository.GetQualificationsAsync().Result;
                if (!qualifications.Any(q => q.QualificationId == model.ParentEducation))
                {
                    _logger.LogError($"Incorrect ParentEducation. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, ParentEducation: {model.ParentEducation}");
                    return this.BadRequest();
                }

                // First Language
                var languages = _languageRepository.GetLanguagesAsync().Result;
                if (!languages.Any(l => l.LanguageId == model.FirstLanguage))
                {
                    _logger.LogError($"Incorrect FirstLanguage. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, FirstLanguage: {model.FirstLanguage}");
                    return this.BadRequest();
                }

                // Second Language
                if (!languages.Any(l => l.LanguageId == model.SecondLanguage))
                {
                    _logger.LogError($"Incorrect SecondLanguage. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, SecondLanguage: {model.SecondLanguage}");
                    return this.BadRequest();
                }

                // Third Language: Optional field
                if (model.ThirdLanguage > 0 && !languages.Any(l => l.LanguageId == model.ThirdLanguage))
                {
                    _logger.LogError($"Incorrect ThirdLanguage. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, ThirdLanguage: {model.ThirdLanguage}");
                    return this.BadRequest();
                }

                var languagesCanSpeak = model.LanguagesCanSpeak?.Trim()?.Split(",");
                if (languagesCanSpeak != null && languagesCanSpeak.Any())
                {
                    foreach (var languageCanSpeak in languagesCanSpeak)
                    {
                        if (!languages.Any(l => l.LanguageId == Convert.ToInt32(languageCanSpeak)))
                        {
                            _logger.LogError($"Incorrect LanguagesCanSpeak. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, MotherTongue: {model.LanguagesCanSpeak}");
                            return this.BadRequest();
                        }
                    }
                }


                // Mother Tongue
                if (!languages.Any(l => l.LanguageId == model.MotherTongue))
                {
                    _logger.LogError($"Incorrect MotherTongue. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, MotherTongue: {model.MotherTongue}");
                    return this.BadRequest();
                }

                var genders = _genderRepository.GetGendersAsync().Result;
                if (!genders.Any(g => g.GenderId == model.GenderId))
                {
                    _logger.LogError($"Incorrect GenderId. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, GenderId: {model.GenderId}");
                    return this.BadRequest();
                }

                var schoolTypes = _schoolTypeRepository.GetSchoolTypesAsync().Result;
                if (!schoolTypes.Any(st => st.SchoolTypeId == model.SchoolTypeId))
                {
                    _logger.LogError($"Incorrect SchoolType. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, SchoolType: {model.SchoolTypeId}");
                    return this.BadRequest();
                }


                var syllabusTypes = _syllabusTypeRepository.GetSyllabusTypesAsync().Result;
                if (!syllabusTypes.Any(st => st.SyllabusTypeId == model.SyllabusTypeId))
                {
                    _logger.LogError($"Incorrect SyllabusType. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, SyllabusType: {model.SyllabusTypeId}");
                    return this.BadRequest();
                }

                int OrganizationStudentId = await _studentRepository.CreateOrganizationStudentAsync
                                        (
                                            organization.OrganizationId,
                                            0, // UserId
                                            model.StudentName,
                                            model.ParentName,
                                            model.Email,
                                            model.PhoneNumber,
                                            model.Zip,
                                            model.LanguagesCanSpeak,
                                            model.DateOfBirth,
                                            model.StudentClass,
                                            model.ParentEducation,
                                            model.FirstLanguage,
                                            model.SecondLanguage,
                                            model.ThirdLanguage,
                                            model.MotherTongue,
                                            model.GenderId,
                                            model.SchoolTypeId,
                                            model.SyllabusTypeId,
                                            0 // model.SectionId
                                        );

                if (OrganizationStudentId < 0)
                { throw new Exception($"Error occured while inserting the OrganizationStudent. OrganizationStudentId: {OrganizationStudentId}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationStudentController.PostAsync(), StudentName: {model.StudentName}, OrganizationOid: {model.OrganizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/OrganizationStudent        
        [HttpPut]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudent_Update)]
        public async Task<IActionResult> PutAsync([FromBody] OrganizationStudentPutModel model)
        {
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1) // Item1: bool
                {
                    _logger.LogError($"{validationResult.Item2}. Source:OrganizationStudentController.PutAsync(), OrganizationOid: {model.OrganizationOid}, StudentId: {model.StudentOid}");
                    return this.BadRequest();
                }

                var organizationStudent = await _studentRepository.GetOrganizationStudentAsync(model.OrganizationOid, model.StudentOid);
                if (organizationStudent == null)
                {
                    _logger.LogError($"OrganizationStudent is null. Source:OrganizationStudentController.PutAsync(), OrganizationOid: {model.OrganizationOid}, StudentId: {model.StudentOid}");
                    return this.BadRequest();
                }

                var studentClasses = _studentClassRepository.GetStudentClassesAsync().Result;
                if (!studentClasses.Any(c => c.ClassId == model.StudentClass))
                {
                    _logger.LogError($"Incorrect StudentClass. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, StudentClass: {model.StudentClass}");
                    return this.BadRequest();
                }

                var qualifications = _qualificationRepository.GetQualificationsAsync().Result;
                if (!qualifications.Any(q => q.QualificationId == model.ParentEducation))
                {
                    _logger.LogError($"Incorrect ParentEducation. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, ParentEducation: {model.ParentEducation}");
                    return this.BadRequest();
                }

                // First Language
                var languages = _languageRepository.GetLanguagesAsync().Result;
                if (!languages.Any(l => l.LanguageId == model.FirstLanguage))
                {
                    _logger.LogError($"Incorrect FirstLanguage. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, FirstLanguage: {model.FirstLanguage}");
                    return this.BadRequest();
                }

                // Second Language
                if (!languages.Any(l => l.LanguageId == model.SecondLanguage))
                {
                    _logger.LogError($"Incorrect SecondLanguage. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, SecondLanguage: {model.SecondLanguage}");
                    return this.BadRequest();
                }

                // Third Language: Optional field
                if (model.ThirdLanguage > 0 && !languages.Any(l => l.LanguageId == model.ThirdLanguage))
                {
                    _logger.LogError($"Incorrect ThirdLanguage. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, ThirdLanguage: {model.ThirdLanguage}");
                    return this.BadRequest();
                }

                var languagesCanSpeak = model.LanguagesCanSpeak?.Trim()?.Split(",");
                if (languagesCanSpeak != null && languagesCanSpeak.Any())
                {
                    foreach (var languageCanSpeak in languagesCanSpeak)
                    {
                        if (!languages.Any(l => l.LanguageId == Convert.ToInt32(languageCanSpeak)))
                        {
                            _logger.LogError($"Incorrect LanguagesCanSpeak. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, MotherTongue: {model.LanguagesCanSpeak}");
                            return this.BadRequest();
                        }
                    }
                }

                // Mother Tongue
                if (!languages.Any(l => l.LanguageId == model.MotherTongue))
                {
                    _logger.LogError($"Incorrect MotherTongue. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, MotherTongue: {model.MotherTongue}");
                    return this.BadRequest();
                }

                var genders = _genderRepository.GetGendersAsync().Result;
                if (!genders.Any(g => g.GenderId == model.GenderId))
                {
                    _logger.LogError($"Incorrect GenderId. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, GenderId: {model.GenderId}");
                    return this.BadRequest();
                }

                var schoolTypes = _schoolTypeRepository.GetSchoolTypesAsync().Result;
                if (!schoolTypes.Any(st => st.SchoolTypeId == model.SchoolTypeId))
                {
                    _logger.LogError($"Incorrect SchoolType. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, SchoolType: {model.SchoolTypeId}");
                    return this.BadRequest();
                }


                var syllabusTypes = _syllabusTypeRepository.GetSyllabusTypesAsync().Result;
                if (!syllabusTypes.Any(st => st.SyllabusTypeId == model.SyllabusTypeId))
                {
                    _logger.LogError($"Incorrect SyllabusType. Source:OrganizationStudentController.PostAsync(), OrganizationOid: {model.OrganizationOid}, SyllabusType: {model.SyllabusTypeId}");
                    return this.BadRequest();
                }

                var organization = await _organizationRepository.GetOrganizationByOidAsync(model.OrganizationOid);
                if (organization == null)
                {
                    _logger.LogError($"Organization is null. Source:OrganizationStudentController.PutAsync(), OrganizationOid: {model.OrganizationOid}");
                    return this.BadRequest();
                }               

                int OrganizationStudentId = await _studentRepository.UpdateOrganizationStudentAsync
                                        (
                                            organizationStudent.StudentId,
                                            organization.OrganizationId,
                                            0, // UserId
                                            model.StudentName,
                                            model.ParentName,
                                            model.Email,
                                            model.PhoneNumber,
                                            model.Zip,
                                            model.LanguagesCanSpeak,
                                            model.DateOfBirth,
                                            model.StudentClass,
                                            model.ParentEducation,
                                            model.FirstLanguage,
                                            model.SecondLanguage,
                                            model.ThirdLanguage,
                                            model.MotherTongue,
                                            model.GenderId,
                                            model.SchoolTypeId,
                                            model.SyllabusTypeId,
                                            model.SectionId,
                                            model.IsActive
                                        );

                if (OrganizationStudentId < 0)
                { throw new Exception($"Error occured while updating the OrganizationStudent. OrganizationStudentId: {OrganizationStudentId}, OrganizationOid:{model.OrganizationOid}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationStudentController.PutAsync(), StudentId: {model.StudentOid}, OrganizationOid: {model.OrganizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // DELETE api/OrganizationStudent/organizationOid/studentId        
        [HttpDelete]
        [AllowedPrivileges(PrivilegeTypes.OrganizationStudent_Delete)]
        public async Task<IActionResult> DeleteAsync(Guid organizationOid, Guid studentOid)
        {
            try
            {
                var organizationStudent = await _studentRepository.GetOrganizationStudentAsync(organizationOid, studentOid);
                if (organizationStudent == null)
                {
                    _logger.LogError($"Organization Student is null. Source:OrganizationStudentController.DeleteAsync(), OrganizationOid: {organizationOid}, StudentOid: {studentOid}");
                    return this.BadRequest();
                }

                int deleteOrganizationStudentResult = await _studentRepository.DeleteOrganizationStudentAsync(organizationStudent.StudentId);
                if (deleteOrganizationStudentResult != 1)
                { throw new Exception($"Error occured while deleting the OrganizationStudent, Result: {deleteOrganizationStudentResult}"); }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source:OrganizationStudentController.DeleteAsync(), StudentOid: {studentOid}, OrganizationOid: {organizationOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
