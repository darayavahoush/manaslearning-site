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
    public class SchoolTypeRepository: ISchoolTypeRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public SchoolTypeRepository(IApplicationConnection applicationConnection, ILogger<SchoolTypeRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<SchoolType>> GetSchoolTypesAsync()
        {
            string procedureName = "[dbo].[GetSchoolTypes]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<SchoolType>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=SchoolTypeRepository.GetSchoolTypesAsync()");
                return null;
            }
        }
    }
}
