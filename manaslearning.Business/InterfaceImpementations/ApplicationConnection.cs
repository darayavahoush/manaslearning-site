using manaslearning.Business.Interfaces;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace manaslearning.Business.InterfaceImpementations
{
    public class ApplicationConnection : IApplicationConnection
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public ApplicationConnection(IConfiguration configuration, ILogger<ApplicationConnection> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
        }

        public SqlConnection GetConnection()
        {
            try
            {
                // EnableRetryLogic
                AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableRetryLogic", true);
                SqlRetryLogicOption retryLogicOption = new SqlRetryLogicOption()
                {
                    NumberOfTries = 6,
                    DeltaTime = TimeSpan.FromSeconds(15),
                    MaxTimeInterval = TimeSpan.FromMinutes(2),
                    TransientErrors = new[] { 10928, 10929, 10053, 10054, 10060, 40197, 40540, 40613, 40615, 40143, 233, 64, 53, -2, 11001 }
                };

                SqlRetryLogicBaseProvider retryLogicBaseProvider = SqlConfigurableRetryFactory.CreateFixedRetryProvider(retryLogicOption);
                var connection = new SqlConnection(_configuration.GetConnectionString("ML"))
                {
                    RetryLogicProvider = retryLogicBaseProvider
                };
                connection.RetryLogicProvider.Retrying += (object sender, SqlRetryingEventArgs args) =>
                {
                    if (args != null && args.Exceptions != null && args.Exceptions.Count > 0)
                    {
                        _logger?.LogError(args.Exceptions[0], $"ErrorMessage={args.Exceptions[0].Message}, Source=ApplicationConnection.GetConnection(), Retry Count: {args.RetryCount}, Delay: {args.Delay}");
                    }
                    else
                    {
                        _logger?.LogInformation($"Source=ApplicationConnection.GetConnection(), Retry Count: {args.RetryCount}, Delay: {args.Delay}");
                    }
                };
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                connection.AccessToken = azureServiceTokenProvider.GetAccessTokenAsync("https://database.windows.net/").Result;
                return connection;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"ErrorMessage={ex.Message}, Source=ApplicationConnection.GetConnection()");
                throw;
            }
        }
    }
}
