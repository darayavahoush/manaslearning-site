using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace manaslearning.UI.Controllers.User
{
    [Route("api/u/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly IQualificationRepository _qualificationRepository;
        private readonly IGenderRepository _genderRepository;
        private readonly ISchoolTypeRepository _schoolTypeRepository;
        private readonly ISyllabusTypeRepository _syllabusTypeRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger _logger;

        public StudentController(
            ILanguageRepository languageRepository,
            IStudentClassRepository studentClassRepository,
            IQualificationRepository qualificationRepository,
            IGenderRepository genderRepository,
            ISchoolTypeRepository schoolTypeRepository,
            ISyllabusTypeRepository syllabusTypeRepository,
            IStudentRepository studentRepository,
            ILogger<StudentController> logger)
        {
            this._languageRepository = languageRepository;
            this._studentClassRepository = studentClassRepository;
            this._qualificationRepository = qualificationRepository;
            this._genderRepository = genderRepository;
            this._schoolTypeRepository = schoolTypeRepository;
            this._syllabusTypeRepository = syllabusTypeRepository;
            this._studentRepository = studentRepository;
            this._logger = logger;
        }

        // GET api/student
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            long _userId = 0;
            int organizationId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                organizationId = HttpContext.GetOrganizationId();
                var loginType = HttpContext.GetLoginType();
                IEnumerable<Student> students;

                if (loginType == Enums.LoginTypes.OrganiztionUser)
                {
                    students = await _studentRepository.GetStudentsByTeacherIdAsync(organizationId, _userId);
                }
                else 
                {
                    students = await _studentRepository.GetStudentsAsync(_userId);
                }
                
                return Ok(students.Select(s => new StudentDto
                {
                    StudentId = s.StudentId,
                    StudentName = s.StudentName,
                    ParentName = s.ParentName,
                    Email = s.Email,
                    DateOfBirth = s.DateOfBirth,
                    PhoneNumber = s.PhoneNumber,
                    StudentClass = s.StudentClassName
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=StudentController.GetAsync();UserId={_userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/student/info?id=2
        [HttpGet("[Action]")]
        public async Task<IActionResult> InfoAsync(int id)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                var studentInfo = await _studentRepository.GetStudentInfoAsync(_userId, id);
                if (studentInfo == null)
                    return this.NotFound();

                studentInfo.LanguagesCanSpeak = GetLanguageNamesForStudentCanSpeak(_languageRepository, studentInfo.LanguagesCanSpeak);
                return Ok(studentInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=StudentController.InfoAsync(), UserId={_userId}, Id={id}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/student/edit?id=2
        [HttpGet("[Action]")]
        public async Task<IActionResult> EditAsync(int id)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                var student = await _studentRepository.GetStudentAsync(_userId, id);
                if (student == null)
                    return this.NotFound();

                return Ok(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=StudentController.EditAsync(), UserId={_userId}, Id={id}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> CountsAsync()
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                var students = await _studentRepository.GetStudentCountByGenderAsync(_userId);
                if (students == null)
                    return this.NotFound();

                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=StudentController.CountsAsync(), UserId={_userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/student
        // TODO: Implement "AntiForgeryToken"
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] StudentPostRequestModel model)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();

                int organizationId = HttpContext.GetOrganizationId();

                if (!IsStudentModelValid(model))
                    return this.BadRequest("Bad Request.");

                // TODO: check for duplicate

                var result = await _studentRepository.CreateOrUpdateStudentAsync(
                        model.RecordId,
                        _userId,
                        organizationId,
                        model.Name,
                        model.ParentName,
                        model.Email,
                        model.Phone,
                        model.Zip,
                        model.StudentCanSpeak,
                        DateTime.Parse(model.Dob),
                        model.StudentClass,
                        model.ParentEducation,
                        model.FirstLanguage,
                        model.SecondLanguage,
                        model.ThirdLanguage,
                        model.MotherTongue,
                        model.Gender,
                        model.SchoolType,
                        model.SyllabusType
                    );

                if (result == -1)
                    return this.StatusCode((int)HttpStatusCode.InternalServerError);
                else if (result == -2)
                    return this.Forbid("Unauthorized request.");
                else if (result == -100)
                    return this.BadRequest("Your account reached maximum students limit. For more details contact support team.");

                return Ok("success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=StudentController.PostAsync();RecordId={model.RecordId},UserId={_userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Determines StudentPostRequestModel model is valid.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        private bool IsStudentModelValid(StudentPostRequestModel model)
        {
            string inputRegex = @"^[\w\s-./]+$";

            // Name
            if (string.IsNullOrWhiteSpace(model.Name) || model.Name.Length > 50 || !Regex.IsMatch(model.Name, inputRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2)))
                return false;

            // Parent Name
            if (string.IsNullOrWhiteSpace(model.ParentName) || model.ParentName.Length > 50 || !Regex.IsMatch(model.ParentName, inputRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2)))
                return false;

            // Email
            string emailRegex = @"^[a-z0-9._%+-]{2,}@[a-z0-9.-]{2,}\.[a-z]{2,3}$";
            if (string.IsNullOrWhiteSpace(model.Email) || model.Email.Length > 256 || !Regex.IsMatch(model.Email, emailRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2)))
                return false;

            // Phone
            // TODO: Phone regex validation
            if (string.IsNullOrWhiteSpace(model.Phone) || model.Phone.Length != 10 || !long.TryParse(model.Phone, out long _phone))
                return false;

            // Zip: 10 char // TODO: Accept numbers only
            if (string.IsNullOrWhiteSpace(model.Zip) || model.Zip.Length < 6 || model.Zip.Length > 10 || !long.TryParse(model.Zip, out long _zip))
                return false;

            // Student can speak 
            // Example: 1,2
            string regexSCS = @"^[0-9,]+$";
            if (string.IsNullOrWhiteSpace(model.StudentCanSpeak) || model.StudentCanSpeak.Length > 500 || !Regex.IsMatch(model.StudentCanSpeak, regexSCS, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2)))
                return false;

            // DOB
            // Must be between 1/1/1950 and (Current Year - 2).
            if (string.IsNullOrWhiteSpace(model.Dob) || !DateTime.TryParse(model.Dob, out DateTime _dob) || _dob.Year < 1950 || _dob.Year > DateTime.Now.Year - 2)
                return false;

            // Student Class
            if (model.StudentClass <= 0)
                return false;

            var studentClasses = _studentClassRepository.GetStudentClassesAsync().Result;
            if (!studentClasses.Any(c => c.ClassId == model.StudentClass))
                return false;

            // Parent Education
            if (model.ParentEducation <= 0)
                return false;

            var qualifications = _qualificationRepository.GetQualificationsAsync().Result;
            if (!qualifications.Any(q => q.QualificationId == model.ParentEducation))
                return false;

            // First Language
            var languages = _languageRepository.GetLanguagesAsync().Result;
            if (model.FirstLanguage <= 0
                || !languages.Any(l => l.LanguageId == model.FirstLanguage))
                return false;

            // Second Language
            if (model.SecondLanguage <= 0
                || !languages.Any(l => l.LanguageId == model.SecondLanguage))
                return false;

            // Third Language: Optional field
            if (model.ThirdLanguage > 0)
            {
                if (!languages.Any(l => l.LanguageId == model.ThirdLanguage))
                    return false;
            }

            // Mother Tongue
            if (model.MotherTongue <= 0
                || !languages.Any(l => l.LanguageId == model.MotherTongue))
                return false;

            // Gender
            if (model.Gender <= 0)
                return false;

            var genders = _genderRepository.GetGendersAsync().Result;
            if (!genders.Any(g => g.GenderId == model.Gender))
                return false;

            // School Type
            if (model.SchoolType <= 0)
                return false;

            var schoolTypes = _schoolTypeRepository.GetSchoolTypesAsync().Result;
            if (!schoolTypes.Any(st => st.SchoolTypeId == model.SchoolType))
                return false;

            // Syllabus Type
            if (model.SyllabusType <= 0)
                return false;

            var syllabusTypes = _syllabusTypeRepository.GetSyllabusTypesAsync().Result;
            if (!syllabusTypes.Any(st => st.SyllabusTypeId == model.SyllabusType))
                return false;

            return true;
        }

        /// <summary>
        /// Get Language Names for LanguagesCanSpeak
        /// Example: input = "4, 19, 21"
        /// </summary>
        /// <param name="languageRepository"></param>
        /// <param name="input"></param>
        /// <returns>string</returns>
        private string GetLanguageNamesForStudentCanSpeak(ILanguageRepository languageRepository, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var languages = languageRepository.GetLanguagesAsync().Result;
            var list = input.Split(',');

            List<string> languageNamesArray = new List<string>();

            foreach (var item in list)
            {
                if (int.TryParse(item, out int _languageId))
                {
                    var language = languages.FirstOrDefault(l => l.LanguageId == _languageId);

                    if (language != null && !string.IsNullOrWhiteSpace(language.LanguageName))
                        languageNamesArray.Add(language.LanguageName);
                }
            }

            if (languageNamesArray.Count == 0)
                return string.Empty;

            return string.Join(", ", languageNamesArray);
        }
    }
}
