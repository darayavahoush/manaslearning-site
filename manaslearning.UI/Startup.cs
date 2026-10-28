using manaslearning.Business;
using manaslearning.Business.InterfaceImpementations;
using manaslearning.Business.Interfaces;
using manaslearning.UI.Helpers.EmailProvider;
using manaslearning.T4;
using manaslearning.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using manaslearning.Payment;

namespace manaslearning.UI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // DI Setup
            // Infrastructure
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // shared
            services.AddScoped<IStorageHelper, StorageHelper>();

            services.AddScoped<IApplicationConnection, ApplicationConnection>();
            services.AddScoped<IFreeAssessmentRepository, FreeAssessmentRepository>();
            services.AddScoped<IFreeAssessmentRepository, FreeAssessmentRepository>();
            services.AddScoped<IContactUsRepository, ContactUsRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();

            // User
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<IGenderRepository, GenderRepository>();
            services.AddScoped<IQualificationRepository, QualificationRepository>();
            services.AddScoped<ISchoolTypeRepository, SchoolTypeRepository>();
            services.AddScoped<ISyllabusTypeRepository, SyllabusTypeRepository>();
            services.AddScoped<IStudentClassRepository, StudentClassRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IAssessmentRepository, AssessmentRepository>();
            services.AddScoped<IStudentAssessmentRepository, StudentAssessmentRepository>();
            services.AddScoped<IAssessmentReportRepository, AssessmentReportRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IAddressTypeRepository, AddressTypeRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentStatusRepository, PaymentStatusRepository>();
            services.AddScoped<ILicenseRepository, LicenseRepository>();
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<ITeacherClassRepository, TeacherClassRepository>();

            services.AddScoped<ITemplateGenerater, TemplateGenerater>();
            services.AddScoped<IPaytmGateway, PaytmGateway>();
            services.AddScoped<ICustomEmailSender, EmailSender>();
            services.AddScoped<IEnquiryRepository, EnquiryRepository>();

            // The following line enables Application Insights telemetry collection.
            services.AddApplicationInsightsTelemetry();

            services.AddControllers();
            services.AddRazorPages();

            // Authentication
            // Google
            services.AddAuthentication().AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection = _configuration.GetSection("Authentication:Google");

                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.CallbackPath = new PathString("/signin-google");

                options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                options.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");
                options.SaveTokens = true;
            });

            // Facebook
            //services.AddAuthentication().AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = _configuration["Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = _configuration["Authentication:Facebook:AppSecret"];
            //    facebookOptions.SignInScheme = IdentityConstants.ExternalScheme;
            //    //facebookOptions.CallbackPath = new PathString("/signin-facebook");
            //});

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(5);

                options.LoginPath = $"/login"; //"/Identity/Account/Login";
                options.LogoutPath = $"/logout"; //"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/accessdenied"; //"/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            // Email
            // Ref:
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-5.0&tabs=visual-studio
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-3.1&tabs=netcore-cli#debug
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(_configuration);
            // Data Protection
            //services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(3));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // handle .webmanifest file
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";

            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = provider,
                OnPrepareResponse = ctx =>
                {
                    // Cache static files for 1 hour
                    ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=7200");
                    ctx.Context.Response.Headers.Add("Expires", DateTime.UtcNow.AddHours(5).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
                }
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
