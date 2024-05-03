using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApiTemplate.REST.Common;
using MinimalApiTemplate.REST.Common.Extensions;

namespace MinimalApiTemplate.REST.Features.Auth.RefreshToken;

public class RefreshTokenEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("refresh-token", RefreshToken);
    }

    public static async Task<IResult> RefreshToken([FromBody] RefreshTokenRequest request, IMediator mediator, TimeProvider timeProvider, HttpContext httpContext)
    {
        request.RefreshToken = httpContext.GetValueFromCookie("refreshToken");
        httpContext.Response.Cookies.Delete("refreshToken");

        var result = await mediator.Send(request);

        if (result.IsSuccess)
        {
            httpContext.AddCookieToResponse("refreshToken", result.Value.RefreshToken, true, timeProvider.GetUtcNow().AddYears(1).DateTime);
            return Results.Ok(new { result.Value.AccessToken });
        }

        return Results.Problem(result.ToProblemDetails());
    }
}
