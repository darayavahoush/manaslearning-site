using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace manaslearning.Auth.API;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;
    public ConfigureJwtBearerOptions(IConfiguration configuration)
    {
        _configuration = configuration;
    }
     
    public void Configure(string name, JwtBearerOptions options)
    {
        RSA rsa = RSA.Create();
        rsa.ImportRSAPublicKey(Convert.FromBase64String(_configuration.GetValue<string>("Jwt:Keys:Public")), out _);

        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidateIssuer = true,
            ValidIssuer = "AuthService",
            ValidateAudience = true,
            ValidAudience = "myApi",
            CryptoProviderFactory = new CryptoProviderFactory()
            {
                CacheSignatureProviders = false
            }
        };
    }

    public void Configure(JwtBearerOptions options)
    {
        throw new NotImplementedException();
    }    
}
