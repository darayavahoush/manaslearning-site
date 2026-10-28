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
    public class CurrencyRepository: ICurrencyRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public CurrencyRepository(IApplicationConnection applicationConnection, ILogger<CurrencyRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<Currency>> GetCurrenciesAsync()
        {
            string procedureName = "[dbo].[GetCurrencies]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    return await connection.QueryAsync<Currency>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=CurrencyRepository.GetCurrenciesAsync()");
                return null;
            }
        }
    }
}
