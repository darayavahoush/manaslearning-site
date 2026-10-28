using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace manaslearning.UI
{
    public class EmailConfirmTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public EmailConfirmTokenProvider(IDataProtectionProvider dataProtectionProvider,
                                         IOptions<EmailConfirmTokenProviderOption> options,
                                         ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
        {
        }
    }
    public class EmailConfirmTokenProviderOption : DataProtectionTokenProviderOptions
    {
        public EmailConfirmTokenProviderOption()
        {
        }

    }
}
