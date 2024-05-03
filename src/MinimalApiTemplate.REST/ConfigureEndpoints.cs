using MinimalApiTemplate.REST.Common;
using MinimalApiTemplate.REST.Features.Auth.ConfirmLogin;
using MinimalApiTemplate.REST.Features.Auth.Login;
using MinimalApiTemplate.REST.Features.Auth.RefreshToken;

namespace MinimalApiTemplate.REST;

public static class ConfigureEndpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("")
            .RequireAuthorization();

        endpoints.MapGroup("auth")
            .AllowAnonymous()
            .MapEndpoint<LoginEndpoint>()
            .MapEndpoint<ConfirmLoginEndpoint>()
            .MapEndpoint<RefreshTokenEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.MapEndpoint(app);
        return app;
    }
}