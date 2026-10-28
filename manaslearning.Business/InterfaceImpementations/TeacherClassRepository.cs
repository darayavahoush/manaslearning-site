using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Helpers;
using manaslearning.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class TeacherClassRepository : ITeacherClassRepository
    {
        private readonly ILogger _logger;
        private readonly IApplicationConnection _applicationConnection;
        public TeacherClassRepository(ILogger<TeacherClassRepository> logger, IApplicationConnection applicationConnection)
        {
            _logger = logger;
            _applicationConnection = applicationConnection;
        }        

        /// <summary>
        /// Get all teacher classes by OrganizationOid
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<TeacherClass>> GetAllTeacherClassesByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetAllTeacherClassesByOrganizationOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid
                    };
                    return await connection.QueryAsync<TeacherClass>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=TeacherClassRepository.GetAllTeacherClassesByOrganizationOidAsync(), OrganizationOid={organizationOid}");
                return null;
            }
        }

        /// <summary>
        /// Get teacher classes by OrganizationId and teacherId
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="teacherId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<TeacherClassInfo>> GetTeacherClassesInfoAsync(int organizationId, long teacherId)
        {
            string procedureName = "[dbo].[GetTeacherClassesInfo]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        TeacherId = teacherId
                    };
                    return await connection.QueryAsync<TeacherClassInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=TeacherClassRepository.GetTeacherClassesInfoAsync(), OrganizationId={organizationId}, TeacherId={teacherId}");
                return null;
            }
        }

        /// <summary>
        /// Get teacher classes by OrganizationOid and teacherId
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <param name="teacherId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<TeacherClass>> GetTeacherClassesAsync(Guid organizationOid, long teacherId)
        {
            string procedureName = "[dbo].[GetTeacherClasses]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid,
                        TeacherId = teacherId
                    };
                    return await connection.QueryAsync<TeacherClass>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=TeacherClassRepository.GetTeacherClassesAsync(), OrganizationOid={organizationOid}, TeacherId={teacherId}");
                return null;
            }
        }

        /// <summary>
        /// Get teacher classes by OrganizationOid
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<TeacherClass>> GetTeacherClassesByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetTeacherClassesByOrganizationOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid
                    };
                    return await connection.QueryAsync<TeacherClass>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=TeacherClassRepository.GetTeacherClassesByOrganizationOidAsync(), OrganizationOid={organizationOid}");
                return null;
            }
        }

        /// <summary>
        /// Get Organization Teacher Class
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <param name="teacherClassOid"></param>
        /// <returns>Teacher</returns>
        public async Task<TeacherClass> GetOrganizationTeacherClassAsync(Guid organizationOid, Guid teacherClassOid)
        {
            string procedureName = "[dbo].[GetOrganizationTeacherClass]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid,
                        TeacherClassOid = teacherClassOid
                    };
                    return await connection.QueryFirstOrDefaultAsync<TeacherClass>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=TeacherClassRepository.GetOrganizationTeacherClassAsync(), OrganizationOid={organizationOid}, TeacherClassOid={teacherClassOid}");
                return null;
            }
        }

        /// <summary>
        /// Get all teacher classes info by OrganizationOid
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<TeacherClassInfo>> GetAllTeacherClassesInfoByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetAllTeacherClassesInfoByOrganizationOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid
                    };
                    return await connection.QueryAsync<TeacherClassInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=TeacherClassRepository.GetAllTeacherClassesInfoByOrganizationOidAsync(), OrganizationOid={organizationOid}");
                return null;
            }
        }

        /// <summary>
        /// Get teacher classes info by OrganizationOid
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<TeacherClassInfo>> GetTeacherClassesInfoByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetTeacherClassesInfoByOrganizationOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid
                    };
                    return await connection.QueryAsync<TeacherClassInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=TeacherClasstRepository.GetTeacherClassesInfoByOrganizationOidAsync(), OrganizationOid={organizationOid}");
                return null;
            }
        }

        /// <summary>
        /// Get Organization Teacher Class Info
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <param name="teacherClassOid"></param>
        /// <returns>Teacher</returns>
        public async Task<TeacherClassInfo> GetOrganizationTeacherClassInfoAsync(Guid organizationOid, Guid teacherClassOid)
        {
            string procedureName = "[dbo].[GetOrganizationTeacherClassInfo]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid,
                        TeacherClassOid = teacherClassOid
                    };
                    return await connection.QueryFirstOrDefaultAsync<TeacherClassInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=TeacherClassRepository.GetOrganizationTeacherClassInfoAsync(), OrganizationOid={organizationOid}, TeacherClassOid={teacherClassOid}");
                return null;
            }
        }

        /// <summary>
        /// Delete Organization Teacher Class
        /// </summary>
        /// <param name="teacherClassOid"></param>
        /// <returns></returns>
        public async Task<int> DeleteOrganizationTeacherClassAsync(Guid teacherClassOid)
        {
            string procedureName = "[dbo].[DeleteOrganizationTeacherClass]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        TeacherClassOid = teacherClassOid
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = TeacherClassRepository.DeleteOrganizationTeacherClassAsync, TeacherClassOid={teacherClassOid}");
                return -1;
            }
        }

        /// <summary>
        /// Create Organization Teacher Class
        /// </summary>
        /// <param name="organizationId"></param>        
        /// <param name="teacherId"></param>
        /// <param name="classId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> CreateOrganizationTeacherClassAsync(
            int organizationId,     
            long teacherId,
            int classId,
            long userId
        )
        {
            string procedureName = "[dbo].[CreateOrganizationTeacherClass]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        TeacherId = teacherId,
                        ClassId = classId,                        
                        UserId = userId
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=TeacherClassRepository.CreateOrganizationTeacherClassAsync(), OrganizationId={organizationId}, UserId={userId}");
                return -1;
            }
        }

        /// <summary>
        /// Update Organization Teacher Class
        /// </summary>
        /// <param name="teacherClassOid"></param>
        /// <param name="teacherId"></param>
        /// <param name="classId"></param>
        /// <param name="organizationId"></param>        
        /// <param name="isActive"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> UpdateOrganizationTeacherClassAsync(
            Guid teacherClassOid,
            long teacherId,
            int classId,
            int organizationId,            
            bool isActive,
            long userId
        )
        {
            string procedureName = "[dbo].[UpdateOrganizationTeacherClass]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        TeacherClassOid = teacherClassOid,
                        TeacherId = teacherId,
                        ClassId = classId,
                        OrganizationId = organizationId,                        
                        IsActive = isActive,
                        UserId = userId
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=TeacherClassRepository.UpdateOrganizationTeacherClassAsync(), OrganizationId={organizationId}, UserId={userId}");
                return -1;
            }
        }

        /// <summary>
        /// Update Teacher Class Mappings
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="classIds"></param>
        /// <param name="organizationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> SaveTeacherClassMappingsAsync(
            long teacherId,
            int[] classIds,
            int organizationId,
            long userId
        )
        {
            string procedureName = "[dbo].[SaveTeacherClassMappings]";
            try
            {
                var dtRelatedLinks = classIds.ConvertToDataTable();
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new 
                    { 
                        TeacherId = teacherId, 
                        ClassIds = dtRelatedLinks.AsTableValuedParameter("TVP_Int"),
                        OrganizationId = organizationId,
                        UserId = userId
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=TeacherClassRepository.SaveTeacherClassMappingsAsync()");
                return -1;
            }
        }
    }
}
