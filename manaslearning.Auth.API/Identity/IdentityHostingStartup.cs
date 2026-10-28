using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;


[assembly: HostingStartup(typeof(manaslearning.Auth.API.IdentityHostingStartup))]
namespace manaslearning.Auth.API
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<manaslearningDbContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("ML"))
                    .AddInterceptors());

                services.AddIdentityCore<IdentityUser>((options) =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Tokens.EmailConfirmationTokenProvider = "EmailConfirmation";
                    options.Tokens.PasswordResetTokenProvider = "PasswordReset";
                })
                .AddEntityFrameworkStores<manaslearningDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<EmailConfirmTokenProvider<IdentityUser>>("EmailConfirmation")
                .AddTokenProvider<PasswordResetTokenProvider<IdentityUser>>("PasswordReset");

                services.Configure<EmailConfirmTokenProviderOption>(options => options.TokenLifespan = TimeSpan.FromMinutes(15));
                services.Configure<PasswordResetTokenProviderOption>(option => option.TokenLifespan = TimeSpan.FromMinutes(15));
            });
        }
    }
}