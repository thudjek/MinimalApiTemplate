using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MinimalApiTemplate.REST.Entities;

namespace MinimalApiTemplate.REST.Features.Auth;

public class LoginTokenProvider : DataProtectorTokenProvider<User>
{
    public static string ProviderName => "LoginTokenProvider";
    public static string Purpose => "Login";
    public LoginTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<DataProtectionTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<User>> logger)
        : base(dataProtectionProvider, options, logger)
    {
        Options.TokenLifespan = TimeSpan.FromMinutes(30);
    }
}
