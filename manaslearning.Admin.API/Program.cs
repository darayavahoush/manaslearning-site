using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using manaslearning.Business.InterfaceImpementations;
using manaslearning.Business.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Adds Microsoft Identity platform (Azure AD) support to protect this Api
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options => { options.TokenValidationParameters.NameClaimType = "name"; });
// End of the Microsoft Identity platform block

// Add services to the container.
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IOrganizationAddressRepository, OrganizationAddressRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IAssessmentRepository, AssessmentRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<ILicenseRepository, LicenseRepository>();
builder.Services.AddScoped<ILicenseHistoryRepository, LicenseHistoryRepository>();

builder.Services.AddScoped<IApplicationConnection, ApplicationConnection>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleCategoryRepository, ArticleCategoryRepository>();
builder.Services.AddScoped<IArticleTagRepository, ArticleTagRepository>();
builder.Services.AddScoped<IArticleRelatedLinksRepository, ArticleRelatedLinksRepository>();
builder.Services.AddScoped<IArticleHistoryRepository, ArticleHistoryRepository>();
builder.Services.AddScoped<IPrivilegeRepository, PrivilegeRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentClassRepository, StudentClassRepository>();
builder.Services.AddScoped<IQualificationRepository, QualificationRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IGenderRepository, GenderRepository>();
builder.Services.AddScoped<ISchoolTypeRepository, SchoolTypeRepository>();
builder.Services.AddScoped<ISyllabusTypeRepository, SyllabusTypeRepository>();
builder.Services.AddScoped<ITeacherClassRepository, TeacherClassRepository>();


// The following line enables Application Insights telemetry collection.
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder
                        .WithOrigins("https://localhost:44383", "https://localhost:7001")
                        .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization)
                        .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                        .AllowCredentials();
                    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
    //options.ExcludedHosts.Add("example.com");
    //options.ExcludedHosts.Add("www.example.com");
});

var app = builder.Build();

string? keyVaultName = null;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    keyVaultName = builder.Configuration["KeyVaultName"];
}
else
{
    app.UseHsts();
    keyVaultName = Environment.GetEnvironmentVariable("KeyVaultName");
}

if (string.IsNullOrWhiteSpace(keyVaultName))
    throw new InvalidOperationException("Store the Key Vault name in a KeyVaultName environment variable.");

var keyVaultEndPoint = new Uri($"https://{keyVaultName}.vault.azure.net/");

var secretClient = new SecretClient(keyVaultEndPoint, new DefaultAzureCredential());
builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());

app.UseHttpsRedirection();

app.UseRouting(); // new

app.UseCors("CorsPolicy");

app.UseAuthentication(); // new

app.UseAuthorization();

//app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();