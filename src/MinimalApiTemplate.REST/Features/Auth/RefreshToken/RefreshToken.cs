using MediatR;
using Microsoft.AspNetCore.Identity;
using MinimalApiTemplate.REST.Common;
using MinimalApiTemplate.REST.Entities;

namespace MinimalApiTemplate.REST.Features.Auth.RefreshToken;

public class RefreshTokenRequest() : IRequest<Result<RefreshTokenResponse>>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public record RefreshTokenResponse(string AccessToken, string RefreshToken);

public class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, Result<RefreshTokenResponse>>
{
    private readonly JwtConfig _jwtConfig;
    private readonly UserManager<User> _userManager;
    private readonly TimeProvider _timeProvider;
    public RefreshTokenRequestHandler(JwtConfig jwtConfig, UserManager<User> userManager, TimeProvider timeProvider)
    {
        _jwtConfig = jwtConfig;
        _userManager = userManager;
        _timeProvider = timeProvider;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var claimsPrincipal = AccessTokenGenerator.GetClaimsPrincipalFromAccessToken(request.AccessToken, _jwtConfig.Secret);

        if (claimsPrincipal == null)
        {
            return Result<RefreshTokenResponse>.Fail("Access or refresh token is expired or invalid");
        }

        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= _timeProvider.GetUtcNow().DateTime)
        {
            return Result<RefreshTokenResponse>.Fail("Access or refresh token is expired or invalid");
        }

        var expiresOn = _timeProvider.GetUtcNow().AddMinutes(_jwtConfig.AccessTokenValidityInMinutes).DateTime;
        var accessToken = AccessTokenGenerator.GenerateAccessToken(user.GetClaims(), _jwtConfig.Secret, _jwtConfig.Issuer, _jwtConfig.Audience, expiresOn);
        var refreshToken = AccessTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = _timeProvider.GetUtcNow().AddDays(_jwtConfig.RefreshTokenValidityInDays).DateTime;

        await _userManager.UpdateSecurityStampAsync(user);

        return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse(accessToken, refreshToken));
    }
}
