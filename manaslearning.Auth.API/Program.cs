using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using manaslearning.Auth.API.Helpers.EmailProvider;
using manaslearning.Business.InterfaceImpementations;
using manaslearning.Business.Interfaces;
using manaslearning.T4;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using static manaslearning.Auth.API.Helpers.EmailProvider.EmailSender;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<ITemplateGenerater, TemplateGenerater>();
builder.Services.AddScoped<ICustomEmailSender, EmailSender>();
builder.Services.AddScoped<IApplicationConnection, ApplicationConnection>();

// The following line enables Application Insights telemetry collection.
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowExternalClient", builder =>
    {
        builder.WithOrigins("*") // TODO: update this
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services
    .AddDbContext<manaslearningDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ML")) // TODO: Read connection from keyvault and use identity
    .AddInterceptors());

builder.Services
    .AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddSignInManager()
    .AddEntityFrameworkStores<manaslearningDbContext>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Auth API",
        Description = "Auth API",
        
    });
});

// Send grid configuration
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

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
    keyVaultName = Environment.GetEnvironmentVariable("KeyVaultName");
}


if (string.IsNullOrWhiteSpace(keyVaultName))
    throw new InvalidOperationException("Store the Key Vault name in a KeyVaultName environment variable.");

var keyVaultEndPoint = new Uri($"https://{keyVaultName}.vault.azure.net/");

var secretClient = new SecretClient(keyVaultEndPoint, new DefaultAzureCredential());
builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());

//app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowExternalClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();