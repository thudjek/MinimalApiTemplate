using MediatR;
using Microsoft.AspNetCore.Identity;
using MinimalApiTemplate.REST.Common;
using MinimalApiTemplate.REST.Entities;

namespace MinimalApiTemplate.REST.Features.Auth.ConfirmLogin;

public record ConfirmLoginRequest(string Email, string ConfirmationToken) : IRequest<Result<ConfirmLoginResponse>>;
public record ConfirmLoginResponse(string AccessToken, string RefreshToken);

public class ConfirmLoginRequestHandler : IRequestHandler<ConfirmLoginRequest, Result<ConfirmLoginResponse>>
{
    private readonly UserManager<User> _userManager;
    private readonly TimeProvider _timeProvider;
    private readonly JwtConfig _jwtConfig;
    private readonly ILogger<ConfirmLoginRequestHandler> _logger;
    public ConfirmLoginRequestHandler(UserManager<User> userManager, TimeProvider timeProvider, JwtConfig jwtConfig, ILogger<ConfirmLoginRequestHandler> logger)
    {
        _userManager = userManager;
        _timeProvider = timeProvider;
        _jwtConfig = jwtConfig;
        _logger = logger;
    }

    public async Task<Result<ConfirmLoginResponse>> Handle(ConfirmLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            _logger.LogError("User not found while trying to confirm login.");
            return Result<ConfirmLoginResponse>.Fail("Email or confirmation token is invalid");
        }

        var isTokenValid = await _userManager.VerifyUserTokenAsync(user, LoginTokenProvider.ProviderName, LoginTokenProvider.Purpose, request.ConfirmationToken);
        if (!isTokenValid)
        {
            return Result<ConfirmLoginResponse>.Fail("Email or confirmation token is invalid");
        }

        var expiresOn = _timeProvider.GetUtcNow().AddMinutes(_jwtConfig.AccessTokenValidityInMinutes).DateTime;
        var accessToken = AccessTokenGenerator.GenerateAccessToken(user.GetClaims(), _jwtConfig.Secret, _jwtConfig.Issuer, _jwtConfig.Audience, expiresOn);
        var refreshToken = AccessTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = _timeProvider.GetUtcNow().AddDays(_jwtConfig.RefreshTokenValidityInDays).DateTime;

        await _userManager.UpdateSecurityStampAsync(user);

        return Result<ConfirmLoginResponse>.Success(new ConfirmLoginResponse(accessToken, refreshToken));
    }
}
