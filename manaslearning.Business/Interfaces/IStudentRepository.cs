using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IStudentRepository
    {
        Task<int> CreateOrUpdateStudentAsync(
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
            );

        Task<IEnumerable<Student>> GetStudentsAsync(long userId);

        Task<IEnumerable<Student>> GetStudentsByTeacherIdAsync(int organizationId, long teacherId);

        Task<Student> GetStudentAsync(long userId, int studentId);

        Task<StudentInfo> GetStudentInfoAsync(long userId, int studentId);

        Task<IEnumerable<GenderwiseStudentCount>> GetStudentCountByGenderAsync(long userId);

        Task<IEnumerable<GenderwiseStudentCount>> GetStudentGenderCountByOrganizationIdAsync(int organizationId);

        Task<IEnumerable<Student>> GetAllOrganizationStudentsByOrganizationOidAsync(Guid organizationOid);

        Task<IEnumerable<Student>> GetOrganizationStudentsByOrganizationOidAsync(Guid organizationOid);

        Task<Student> GetOrganizationStudentAsync(Guid organizationOid, Guid studentOid);

        Task<int> CreateOrganizationStudentAsync(
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
        );

        Task<int> UpdateOrganizationStudentAsync(
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
            bool IsActive
        );

        Task<int> DeleteOrganizationStudentAsync(int studentId);
    }
}
