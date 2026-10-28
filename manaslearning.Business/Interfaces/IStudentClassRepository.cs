using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IStudentClassRepository
    {
        Task<IEnumerable<StudentClass>> GetStudentClassesAsync();

        Task<IEnumerable<StudentClass>> GetStudentClassesByTeacherIdAsync(int organizationId, long teacherId);

        Task<IEnumerable<StudentClass>> GetAllStudentClassesByOrganizationOidAsync(Guid organizationOid);

        Task<IEnumerable<StudentClass>> GetAllStudentClassesByOrganizationIdAsync(int organizationId);

        Task<IEnumerable<StudentClass>> GetStudentClassesByOrganizationIdAsync(int organizationId);

        Task<IEnumerable<StudentClass>> GetStudentClassesByOrganizationOidAsync(Guid organizationOid);

        Task<StudentClass> GetOrganizationStudentClassAsync(Guid organizationOid, Guid studentClassOid);

        Task<StudentClass> GetStudentClassByOrganizationIdAsync(int organizationId, Guid studentClassOid);

        Task<int> DeleteOrganizationStudentClassAsync(int studentClassId);

        Task<int> CreateOrganizationStudentClassAsync(
            int organizationId,
            string className,
            string classDescription,
            long userId
        );

        Task<int> UpdateOrganizationStudentClassAsync(
            int classId,
            int organizationId,
            string className,
            string classDescription,
            bool isActive,
            long userId
        );
    }
}
