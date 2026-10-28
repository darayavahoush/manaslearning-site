using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User
{
    [Authorize]
    public class StudentsModel : PageModel
    {
        private readonly ILogger _logger;
        private readonly ILanguageRepository _languageRepository;
        private readonly IGenderRepository _genderRepository;
        private readonly IQualificationRepository _qualificationRepository;
        private readonly ISchoolTypeRepository _schoolTypeRepository;
        private readonly ISyllabusTypeRepository _syllabusTypeRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly ITeacherClassRepository _teacherClassRepository;

        private long userId;
        private int organizationId;
        private Enums.LoginTypes loginType;
        public IEnumerable<Language> languages;
        public IEnumerable<Gender> genders;
        public IEnumerable<Qualification> qualifications;
        public IEnumerable<SchoolType> schoolTypes;
        public IEnumerable<SyllabusType> syllabusTypes;
        public IEnumerable<StudentClass> studentClasses;
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }

        public StudentsModel(
            ILogger<StudentsModel> logger,
            ILanguageRepository languageRepository,
            IGenderRepository genderRepository,
            IQualificationRepository qualificationRepository,
            ISchoolTypeRepository schoolTypeRepository,
            ISyllabusTypeRepository syllabusTypeRepository,
            IStudentClassRepository studentClassRepository,
            ITeacherClassRepository teacherClassRepository
        )
        {
            _logger = logger;
            _languageRepository = languageRepository;
            _genderRepository = genderRepository;
            _qualificationRepository = qualificationRepository;
            _schoolTypeRepository = schoolTypeRepository;
            _syllabusTypeRepository = syllabusTypeRepository;
            _studentClassRepository = studentClassRepository;
            _teacherClassRepository = teacherClassRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                userId = HttpContext.GetUserId();
                if (userId <= 0) return LocalRedirect(UrlConstants.UNAUTHORIZED);

                organizationId = HttpContext.GetOrganizationId();
                if (organizationId <= 0) return LocalRedirect(UrlConstants.UNAUTHORIZED);

               loginType  = HttpContext.GetLoginType();

                languages = await _languageRepository.GetLanguagesAsync();
                if (languages == null)
                {
                    _logger.LogError("Error=Languages not found in database,Source=StudentsModel.OnGetAsync()");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }
                languages = languages?.OrderBy(l => l.LanguageName);

                genders = await _genderRepository.GetGendersAsync();
                if (genders == null)
                {
                    _logger.LogError("Error=Genders not found in database,Source=StudentsModel.OnGetAsync()");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }
                genders = genders?.OrderBy(g => g.GenderName);

                qualifications = await _qualificationRepository.GetQualificationsAsync();
                if (qualifications == null)
                {
                    _logger.LogError("Error=Qualifications not found in database,Source=StudentsModel.OnGetAsync()");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }
                qualifications = qualifications?.OrderBy(q => q.QualificationName);

                schoolTypes = await _schoolTypeRepository.GetSchoolTypesAsync();
                if (schoolTypes == null)
                {
                    _logger.LogError("Error=School types not found in database,Source=StudentsModel.OnGetAsync()");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }
                schoolTypes = schoolTypes?.OrderBy(st => st.SchoolTypeName);

                syllabusTypes = await _syllabusTypeRepository.GetSyllabusTypesAsync();
                if (syllabusTypes == null)
                {
                    _logger.LogError("Error=Syllabus types not found in database,Source=StudentsModel.OnGetAsync()");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }
                syllabusTypes = syllabusTypes?.OrderBy(s => s.SyllabusTypeName);

                if (loginType == Enums.LoginTypes.OrganiztionUser)
                {
                    studentClasses = await _studentClassRepository.GetStudentClassesByTeacherIdAsync(organizationId, userId);
                }
                else 
                {
                    studentClasses = await _studentClassRepository.GetStudentClassesAsync();
                }
                
                if (studentClasses == null)
                {
                    _logger.LogError("Error=Student Classes not found in database,Source=StudentsModel.OnGetAsync()");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }
                studentClasses = studentClasses?.OrderBy(s => s.ClassName);
                SetBreadcrumb();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error: {ex.Message}, UserId={userId}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private void SetBreadcrumb()
        {
            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = new BreadcrumbItem[] {
                    new BreadcrumbItem{ Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                    new BreadcrumbItem{ Title = "Students" }
                },
                IsResponsive = true
            };
        }
    }
}