using manaslearning.UI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: HostingStartup(typeof(manaslearning.UI.Areas.Identity.IdentityHostingStartup))]
namespace manaslearning.UI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<manaslearningContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("ML"))
                    .AddInterceptors());

                services.AddDefaultIdentity<IdentityUser>((options) =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Tokens.EmailConfirmationTokenProvider = "EmailConfirmation";
                    options.Tokens.PasswordResetTokenProvider = "PasswordReset";
                })//.AddDefaultUI()
                .AddEntityFrameworkStores<manaslearningContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<EmailConfirmTokenProvider<IdentityUser>>("EmailConfirmation")
                .AddTokenProvider<PasswordResetTokenProvider<IdentityUser>>("PasswordReset");

                services.Configure<EmailConfirmTokenProviderOption>(options => options.TokenLifespan = TimeSpan.FromMinutes(15));
                services.Configure<PasswordResetTokenProviderOption>(option => option.TokenLifespan = TimeSpan.FromMinutes(15));
            });
        }
    }
}