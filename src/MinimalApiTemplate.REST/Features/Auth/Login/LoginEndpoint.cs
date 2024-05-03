using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApiTemplate.REST.Common;
using MinimalApiTemplate.REST.Common.Extensions;
using MinimalApiTemplate.REST.Common.Filters;

namespace MinimalApiTemplate.REST.Features.Auth.Login;

public class LoginEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("login", Login)
            .AddEndpointFilter<RequestLoggingFilter>()
            .AddEndpointFilter(new AuthorizedUserFilter(Results.Ok()))
            .WithValidation<LoginRequest>();
    }

    private static async Task<IResult> Login([FromBody] LoginRequest request, IMediator mediator)
    {
        await mediator.Send(request);
        return Results.Ok(new { message = "Email with login link will be sent to your email address" });
    }
}