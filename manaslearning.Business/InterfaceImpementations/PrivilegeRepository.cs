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
    public class PrivilegeRepository : IPrivilegeRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public PrivilegeRepository(
            IApplicationConnection applicationConnection,
            ILogger<PrivilegeRepository> logger)
        {
            this._applicationConnection = applicationConnection;
            this._logger = logger;
        }

        public async Task<IEnumerable<Privilege>> GetUserPrivilegesByEmailAsync(string email)
        {
            string procedureName = "[Admin].[GetUserPrivilegesByEmail]";

            try
            {
                await using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<Privilege>(
                        sql: procedureName,
                        param: new { Email = email },
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=PrivilegeRepository.GetUserPrivilegesByEmailAsync, email={email}.");
                return null;
            }
        }
    }
}
