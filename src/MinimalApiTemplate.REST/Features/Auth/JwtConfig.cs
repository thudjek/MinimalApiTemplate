namespace MinimalApiTemplate.REST.Features.Auth;

public class JwtConfig
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenValidityInMinutes { get; set; }
    public int RefreshTokenValidityInDays { get; set; }
}
