using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public StudentRepository(IApplicationConnection applicationConnection, ILogger<StudentRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        /// <summary>
        /// Create or update a student
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <param name="studentName"></param>
        /// <param name="parentName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="zip"></param>
        /// <param name="languagesCanSpeak"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="studentClass"></param>
        /// <param name="parentEducation"></param>
        /// <param name="firstLanguage"></param>
        /// <param name="secondLanguage"></param>
        /// <param name="thirdLanguage"></param>
        /// <param name="motherTongueId"></param>
        /// <param name="genderId"></param>
        /// <param name="schoolTypeId"></param>
        /// <param name="syllabusTypeId"></param>
        /// <returns>
        /// int
        ///     -1:     Error
        ///     -2:     Unauthorized to update student info.
        ///     -100:   Maximum students limit reached.
        /// </returns>
        public async Task<int> CreateOrUpdateStudentAsync(
            int studentId,
            long userId,
            int organizationId,
            string studentName,
            string parentName,
            string email,
            string phoneNumber,
            string zip,
            string languagesCanSpeak,
            DateTime dateOfBirth,
            int studentClass,
            int parentEducation,
            int firstLanguage,
            int secondLanguage,
            int thirdLanguage,
            int motherTongueId,
            int genderId,
            int schoolTypeId,
            int syllabusTypeId
        )
        {
            string procedureName = "[dbo].[CreateOrUpdateStudent]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        StudentId = studentId,
                        UserId = userId,
                        OrganizationId = organizationId,
                        StudentName = studentName,
                        ParentName = parentName,
                        Email = email,
                        PhoneNumber = phoneNumber,
                        Zip = zip,
                        LanguagesCanSpeak = languagesCanSpeak,
                        DateOfBirth = dateOfBirth,
                        StudentClass = studentClass,
                        ParentEducation = parentEducation,
                        FirstLanguage = firstLanguage,
                        SecondLanguage = secondLanguage,
                        ThirdLanguage = thirdLanguage,
                        MotherTongueId = motherTongueId,
                        GenderId = genderId,
                        SchoolTypeId = schoolTypeId,
                        SyllabusTypeId = syllabusTypeId
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentRepository.CreateOrUpdateStudentAsync(), StudentId={studentId}, UserId={userId}");
                return -1;
            }
        }

        /// <summary>
        /// Get all students
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Student>> GetStudentsAsync(long userId)
        {
            string procedureName = "[dbo].[GetStudents]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId
                    };
                    return await connection.QueryAsync<Student>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentRepository.GetStudentsAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get students by teacherId
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="teacherId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Student>> GetStudentsByTeacherIdAsync(int organizationId, long teacherId)
        {
            string procedureName = "[dbo].[GetStudentsByTeacherId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        TeacherId = teacherId
                    };
                    return await connection.QueryAsync<Student>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentRepository.GetStudentsByTeacherIdAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get student
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="studentId"></param>
        /// <returns>Student</returns>
        public async Task<Student> GetStudentAsync(long userId, int studentId)
        {
            string procedureName = "[dbo].[GetStudent]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        StudentId = studentId
                    };
                    return await connection.QueryFirstOrDefaultAsync<Student>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentRepository.GetStudentAsync(), UserId={userId}, StudentId={studentId}");
                return null;
            }
        }

        /// <summary>
        /// Get student Info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="studentId"></param>
        /// <returns>StudentInfo</returns>
        public async Task<StudentInfo> GetStudentInfoAsync(long userId, int studentId)
        {
            string procedureName = "[dbo].[GetStudentInfo]"; // Note: This procedure is used in reporting function
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        UserId = userId,
                        StudentId = studentId
                    };
                    return await connection.QueryFirstOrDefaultAsync<StudentInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentRepository.GetStudentInfoAsync(), UserId={userId}, StudentId={studentId}");
                return null;
            }
        }

        /// <summary>
        /// Get student count by gender
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<GenderwiseStudentCount>> GetStudentCountByGenderAsync(long userId)
        {
            string procedureName = "[dbo].[GetStudentCountByGender]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { UserId = userId };
                    return await connection.QueryAsync<GenderwiseStudentCount>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=GenderRepository.GetGenderwiseStudentsAsync(), UserId={userId}");
                return null;
            }
        }


        /// <summary>
        /// Get student count by gender
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<GenderwiseStudentCount>> GetStudentGenderCountByOrganizationIdAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetStudentGenderCountByOrganizationId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { OrganizationId = organizationId };
                    return await connection.QueryAsync<GenderwiseStudentCount>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=GenderRepository.GetStudentGenderCountByOrganizationIdAsync(), OrganizationId={organizationId}");
                return null;
            }
        }

        /// <summary>
        /// Get all students by OrganizationOid
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Student>> GetAllOrganizationStudentsByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetAllOrganizationStudentsByOrganizationOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid
                    };
                    return await connection.QueryAsync<Student>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentRepository.GetAllOrganizationStudentsByOrganizationOidAsync(), OrganizationOid={organizationOid}");
                return null;
            }
        }

        /// <summary>
        /// Get all students by OrganizationOid
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Student>> GetOrganizationStudentsByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetOrganizationStudentsByOrganizationOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid
                    };
                    return await connection.QueryAsync<Student>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentRepository.GetOrganizationStudentsByOrganizationOidAsync(), OrganizationOid:{organizationOid}");
                return null;
            }
        }

        /// <summary>
        /// Get Organization Student
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <param name="studentOid"></param>
        /// <returns>Student</returns>
        public async Task<Student> GetOrganizationStudentAsync(Guid organizationOid, Guid studentOid)
        {
            string procedureName = "[dbo].[GetOrganizationStudent]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid,
                        StudentOid = studentOid
                    };
                    return await connection.QueryFirstOrDefaultAsync<Student>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentRepository.GetOrganizationStudentAsync(), OrganizationOid={organizationOid}, StudentOid={studentOid}");
                return null;
            }
        }



        /// <summary>
        /// Delete Organization Student
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task<int> DeleteOrganizationStudentAsync(int studentId)
        {
            string procedureName = "[dbo].[DeleteOrganizationStudent]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        StudentId = studentId
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = StudentRepository.DeleteOrganizationStudentAsync, StudentId={studentId}");
                return -1;
            }
        }

        /// <summary>
        /// Create Organization Student
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="userId"></param>
        /// <param name="studentName"></param>
        /// <param name="parentName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="zip"></param>
        /// <param name="languagesCanSpeak"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="studentClass"></param>
        /// <param name="parentEducation"></param>
        /// <param name="firstLanguage"></param>
        /// <param name="secondLanguage"></param>
        /// <param name="thirdLanguage"></param>
        /// <param name="motherTongueId"></param>
        /// <param name="genderId"></param>
        /// <param name="schoolTypeId"></param>
        /// <param name="syllabusTypeId"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public async Task<int> CreateOrganizationStudentAsync(
            int organizationId,
            long userId,
            string studentName,
            string parentName,
            string email,
            string phoneNumber,
            string zip,
            string languagesCanSpeak,
            DateTime dateOfBirth,
            int studentClass,
            int parentEducation,
            int firstLanguage,
            int secondLanguage,
            int thirdLanguage,
            int motherTongueId,
            int genderId,
            int schoolTypeId,
            int syllabusTypeId,
            int sectionId
        )
        {
            string procedureName = "[dbo].[CreateOrganizationStudent]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        UserId = userId,
                        StudentName = studentName,
                        ParentName = parentName,
                        Email = email,
                        PhoneNumber = phoneNumber,
                        Zip = zip,
                        LanguagesCanSpeak = languagesCanSpeak,
                        DateOfBirth = dateOfBirth,
                        StudentClass = studentClass,
                        ParentEducation = parentEducation,
                        FirstLanguage = firstLanguage,
                        SecondLanguage = secondLanguage,
                        ThirdLanguage = thirdLanguage,
                        MotherTongueId = motherTongueId,
                        GenderId = genderId,
                        SchoolTypeId = schoolTypeId,
                        SyllabusTypeId = syllabusTypeId,
                        SectionId = sectionId
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentRepository.CreateOrganizationStudentAsync(), OrganizationId={organizationId}, UserId={userId}");
                return -1;
            }
        }

        /// <summary>
        /// Update Organization Student
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="organizationId"></param>
        /// <param name="userId"></param>
        /// <param name="studentName"></param>
        /// <param name="parentName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="zip"></param>
        /// <param name="languagesCanSpeak"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="studentClass"></param>
        /// <param name="parentEducation"></param>
        /// <param name="firstLanguage"></param>
        /// <param name="secondLanguage"></param>
        /// <param name="thirdLanguage"></param>
        /// <param name="motherTongueId"></param>
        /// <param name="genderId"></param>
        /// <param name="schoolTypeId"></param>
        /// <param name="syllabusTypeId"></param>
        /// <param name="sectionId"></param>
        /// <param name="IsActive"></param>
        /// <returns></returns>
        public async Task<int> UpdateOrganizationStudentAsync(
            int studentId,
            int organizationId,
            long userId,
            string studentName,
            string parentName,
            string email,
            string phoneNumber,
            string zip,
            string languagesCanSpeak,
            DateTime dateOfBirth,
            int studentClass,
            int parentEducation,
            int firstLanguage,
            int secondLanguage,
            int thirdLanguage,
            int motherTongueId,
            int genderId,
            int schoolTypeId,
            int syllabusTypeId,
            int sectionId,
            bool isActive
        )
        {
            string procedureName = "[dbo].[UpdateOrganizationStudent]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        StudentId = studentId,
                        OrganizationId = organizationId,
                        UserId = userId,
                        StudentName = studentName,
                        ParentName = parentName,
                        Email = email,
                        PhoneNumber = phoneNumber,
                        Zip = zip,
                        LanguagesCanSpeak = languagesCanSpeak,
                        DateOfBirth = dateOfBirth,
                        StudentClass = studentClass,
                        ParentEducation = parentEducation,
                        FirstLanguage = firstLanguage,
                        SecondLanguage = secondLanguage,
                        ThirdLanguage = thirdLanguage,
                        MotherTongueId = motherTongueId,
                        GenderId = genderId,
                        SchoolTypeId = schoolTypeId,
                        SyllabusTypeId = syllabusTypeId,
                        SectionId = sectionId,
                        IsActive = isActive
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentRepository.UpdateOrganizationStudentAsync(), OrganizationId={organizationId}, UserId={userId}, StudentId={studentId}");
                return -1;
            }
        }
    }
}
