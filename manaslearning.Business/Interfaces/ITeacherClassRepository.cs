using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface ITeacherClassRepository
    {
        
        Task<IEnumerable<TeacherClass>> GetAllTeacherClassesByOrganizationOidAsync(Guid organizationOid);

        Task<IEnumerable<TeacherClass>> GetTeacherClassesAsync(Guid organizationOid, long teacherId);

        Task<IEnumerable<TeacherClassInfo>> GetTeacherClassesInfoAsync(int organizationId, long teacherId);

        Task<IEnumerable<TeacherClass>> GetTeacherClassesByOrganizationOidAsync(Guid organizationOid);

        Task<TeacherClass> GetOrganizationTeacherClassAsync(Guid organizationOid, Guid teacherClassOid);

        Task<IEnumerable<TeacherClassInfo>> GetAllTeacherClassesInfoByOrganizationOidAsync(Guid organizationOid);

        Task<IEnumerable<TeacherClassInfo>> GetTeacherClassesInfoByOrganizationOidAsync(Guid organizationOid);

        Task<TeacherClassInfo> GetOrganizationTeacherClassInfoAsync(Guid organizationOid, Guid teacherClassOid);

        Task<int> DeleteOrganizationTeacherClassAsync(Guid teacherClassOid);
        
        Task<int> CreateOrganizationTeacherClassAsync(
            int organizationId,
            long teacherId,
            int classId,
            long userId
        );

        Task<int> UpdateOrganizationTeacherClassAsync(
            Guid teacherClassOid,
            long teacherId,
            int classId,
            int organizationId,
            bool isActive,
            long userId
        );

        Task<int> SaveTeacherClassMappingsAsync(            
            long teacherId,
            int[] classIds,
            int organizationId,            
            long userId
        );
    }
}
