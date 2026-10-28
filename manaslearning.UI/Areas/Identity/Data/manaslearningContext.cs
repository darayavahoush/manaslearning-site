using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace manaslearning.UI.Data
{
    public class manaslearningContext : IdentityDbContext<IdentityUser>
    {
        public manaslearningContext()
        {
        }

        public manaslearningContext(DbContextOptions<manaslearningContext> options) : base(options)
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
                var connection = (SqlConnection)Database.GetDbConnection();
                connection.RetryLogicProvider = retryLogicBaseProvider;
                connection.RetryLogicProvider.Retrying += (object sender, SqlRetryingEventArgs args) =>
                {
                    if (args != null && args.Exceptions != null && args.Exceptions.Count > 0)
                    {
                        // TODO: _logger?.LogError(args.Exceptions[0], $"ErrorMessage={args.Exceptions[0].Message}, Source=ApplicationConnection.GetConnection(), Retry Count: {args.RetryCount}, Delay: {args.Delay}");
                    }
                    else
                    {
                        // TODO: _logger?.LogInformation($"Source=ApplicationConnection.GetConnection(), Retry Count: {args.RetryCount}, Delay: {args.Delay}");
                    }
                };

                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                connection.AccessToken = azureServiceTokenProvider.GetAccessTokenAsync("https://database.windows.net/").Result;
            }
            catch (Exception)
            {
                // TODO: log exception
                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
