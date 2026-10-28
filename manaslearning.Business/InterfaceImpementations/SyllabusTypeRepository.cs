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
    public class SyllabusTypeRepository: ISyllabusTypeRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public SyllabusTypeRepository(IApplicationConnection applicationConnection, ILogger<SyllabusTypeRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<SyllabusType>> GetSyllabusTypesAsync()
        {
            string procedureName = "[dbo].[GetSyllabusTypes]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<SyllabusType>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=SyllabusTypeRepository.GetSyllabusTypesAsync()");
                return null;
            }
        }
    }
}
