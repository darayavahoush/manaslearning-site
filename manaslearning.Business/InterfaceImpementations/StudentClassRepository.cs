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
    public class StudentClassRepository : IStudentClassRepository
    {
        private readonly ILogger _logger;
        private readonly IApplicationConnection _applicationConnection;
        public StudentClassRepository(ILogger<StudentClassRepository> logger, IApplicationConnection applicationConnection)
        {
            _logger = logger;
            _applicationConnection = applicationConnection;
        }

        public async Task<IEnumerable<StudentClass>> GetStudentClassesAsync()
        {
            var procedureName = "[dbo].[GetStudentClasses]";

            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<StudentClass>(procedureName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message},Source=StudentClassRepository.GetStudentClassesAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get student classes by TeacherId
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="teacherId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<StudentClass>> GetStudentClassesByTeacherIdAsync(int organizationId, long teacherId)
        {
            string procedureName = "[dbo].[GetStudentClassesByTeacherId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        TeacherId = teacherId
                    };
                    return await connection.QueryAsync<StudentClass>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentClassRepository.GetStudentClassesByTeacherIdAsync(), OrganizationId={organizationId},TeacherId={teacherId}");
                return null;
            }
        }

        /// <summary>
        /// Get all student classes by OrganizationOid
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<StudentClass>> GetAllStudentClassesByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetAllStudentClassesByOrganizationOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid
                    };
                    return await connection.QueryAsync<StudentClass>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentClassRepository.GetAllStudentClassesByOrganizationOidAsync(), OrganizationOid={organizationOid}");
                return null;
            }
        }

        /// <summary>
        /// Get all student classes by OrganizationId
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<StudentClass>> GetAllStudentClassesByOrganizationIdAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetAllStudentClassesByOrganizationId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId
                    };
                    return await connection.QueryAsync<StudentClass>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentClassRepository.GetAllStudentClassesByOrganizationIdAsync(), OrganizationId={organizationId}");
                return null;
            }
        }

        /// <summary>
        /// Get student classes by OrganizationOid
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<StudentClass>> GetStudentClassesByOrganizationOidAsync(Guid organizationOid)
        {
            string procedureName = "[dbo].[GetStudentClassesByOrganizationOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid
                    };
                    return await connection.QueryAsync<StudentClass>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentClasstRepository.GetStudentClassesByOrganizationOidAsync(), OrganizationOid={organizationOid}");
                return null;
            }
        }

        /// <summary>
        /// Get student classes by OrganizationOid
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<StudentClass>> GetStudentClassesByOrganizationIdAsync(int organizationId)
        {
            string procedureName = "[dbo].[GetStudentClassesByOrganizationId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId
                    };
                    return await connection.QueryAsync<StudentClass>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentClasstRepository.GetStudentClassesByOrganizationIdAsync(), OrganizationId={organizationId}");
                return null;
            }
        }

        /// <summary>
        /// Get Organization Student Class
        /// </summary>
        /// <param name="organizationOid"></param>
        /// <param name="studentClassOid"></param>
        /// <returns>Student</returns>
        public async Task<StudentClass> GetOrganizationStudentClassAsync(Guid organizationOid, Guid studentClassOid)
        {
            string procedureName = "[dbo].[GetOrganizationStudentClass]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationOid = organizationOid,
                        StudentClassOid = studentClassOid
                    };
                    return await connection.QueryFirstOrDefaultAsync<StudentClass>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentClassRepository.GetOrganizationStudentClassAsync(), OrganizationOid={organizationOid}, StudentClassOid={studentClassOid}");
                return null;
            }
        }

        /// <summary>
        /// Get Organization Student Class
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="studentClassOid"></param>
        /// <returns>Student</returns>
        public async Task<StudentClass> GetStudentClassByOrganizationIdAsync(int organizationId, Guid studentClassOid)
        {
            string procedureName = "[dbo].[GetOrganizationStudentClassByOrganizationId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,
                        StudentClassOid = studentClassOid
                    };
                    return await connection.QueryFirstOrDefaultAsync<StudentClass>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentClassRepository.GetStudentClassByOrganizationIdAsync(), OrganizationId={organizationId}, StudentClassOid={studentClassOid}");
                return null;
            }
        }
                

        /// <summary>
        /// Delete Organization Student Class
        /// </summary>
        /// <param name="studentClassId"></param>
        /// <returns></returns>
        public async Task<int> DeleteOrganizationStudentClassAsync(int studentClassId)
        {
            string procedureName = "[dbo].[DeleteOrganizationStudentClass]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        StudentClassId = studentClassId
                    };
                    return await connection.ExecuteScalarAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, Source = StudentClassRepository.DeleteOrganizationStudentClassAsync, StudentClassId={studentClassId}");
                return -1;
            }
        }

        /// <summary>
        /// Create Organization Student Class
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="className"></param>
        /// <param name="classDescription"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> CreateOrganizationStudentClassAsync(
            int organizationId,            
            string className,
            string classDescription,
            long userId
        )
        {
            string procedureName = "[dbo].[CreateOrganizationStudentClass]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        OrganizationId = organizationId,                        
                        ClassName = className,
                        ClassDescription = classDescription,
                        UserId = userId
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentClassRepository.CreateOrganizationStudentClassAsync(), OrganizationId={organizationId}, UserId={userId}");
                return -1;
            }
        }

        /// <summary>
        /// Update Organization Student Class
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="organizationId"></param>
        /// <param name="className"></param>
        /// <param name="classDescription"></param>
        /// <param name="isActive"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> UpdateOrganizationStudentClassAsync(
            int classId,
            int organizationId,
            string className,
            string classDescription,
            bool isActive,
            long userId
        )
        {
            string procedureName = "[dbo].[UpdateOrganizationStudentClass]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        ClassId = classId,
                        OrganizationId = organizationId,
                        ClassName = className,
                        ClassDescription = classDescription,
                        IsActive = isActive,
                        UserId = userId
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentClassRepository.UpdateOrganizationStudentClassAsync(), OrganizationId={organizationId}, UserId={userId}");
                return -1;
            }
        }
    }
}
