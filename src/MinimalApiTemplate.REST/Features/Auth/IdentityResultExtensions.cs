using Microsoft.AspNetCore.Identity;
using MinimalApiTemplate.REST.Common.Exceptions;

namespace MinimalApiTemplate.REST.Features.Auth;

public static class IdentityResultExtensions
{
    public static void ThrowIfNotSuccessful(this IdentityResult identityResult, string message = null)
    {
        if (!identityResult.Succeeded)
        {
            if (message != null)
            {
                throw new IdentityException(message, identityResult.Errors?.Select(e => e.Description));
            }

            throw new IdentityException(identityResult.Errors?.Select(e => e.Description));
        }
    }
}
