using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using manaslearning.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace manaslearning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => 
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // KeyVault Setup
                    string? keyVaultName = null;
                    if (!context.HostingEnvironment.IsDevelopment())
                    {
                        keyVaultName = Environment.GetEnvironmentVariable("KeyVaultName");
                    }
                    else
                    {
                        var builtConfig = config.Build();
                        keyVaultName = builtConfig.GetValue<string>("KeyVaultName");
                    }

                    if (string.IsNullOrWhiteSpace(keyVaultName))
                        throw new InvalidOperationException("Store the Key Vault name in a KeyVaultName environment variable.");

                    var keyVaultEndPoint = new Uri($"https://{keyVaultName}.vault.azure.net/");

                    var secretClient = new SecretClient(keyVaultEndPoint, new DefaultAzureCredential());
                    config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
