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
    public class AddressTypeRepository: IAddressTypeRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public AddressTypeRepository(IApplicationConnection applicationConnection, ILogger<AddressTypeRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<AddressType>> GetAddressTypesAsync()
        {
            string procedureName = "[dbo].[GetAddressTypes]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<AddressType>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=AddressTypeRepository.GetAddressTypesAsync()");
                return null;
            }
        }
    }
}
