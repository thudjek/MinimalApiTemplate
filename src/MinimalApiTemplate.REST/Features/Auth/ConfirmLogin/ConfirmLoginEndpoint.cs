using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApiTemplate.REST.Common;
using MinimalApiTemplate.REST.Common.Extensions;
using MinimalApiTemplate.REST.Common.Filters;

namespace MinimalApiTemplate.REST.Features.Auth.ConfirmLogin;

public class ConfirmLoginEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("confirm-login", ConfirmLogin)
            .AddEndpointFilter(new AuthorizedUserFilter(Results.Ok())); ;
    }

    private static async Task<IResult> ConfirmLogin([FromBody] ConfirmLoginRequest request, IMediator mediator, TimeProvider timeProvider, HttpContext httpContext)
    {
        var result = await mediator.Send(request);

        if (result.IsSuccess)
        {
            httpContext.Response.Cookies.Delete("refreshToken");
            httpContext.AddCookieToResponse("refreshToken", result.Value.RefreshToken, true, timeProvider.GetUtcNow().AddYears(1).DateTime);
            return Results.Ok(new { result.Value.AccessToken });
        }

        return Results.Problem(result.ToProblemDetails());
    }
}
