using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MinimalApiTemplate.REST.Entities;

public class User : User<int>
{
}

public class User<TKey> : IdentityUser<TKey> where TKey : IEquatable<TKey>
{
    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public List<Claim> GetClaims()
    {
        return new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
            new Claim(ClaimTypes.Email, Email)
        };
    }
}