namespace MinimalApiTemplate.REST.Common.Filters;

public class AuthorizedUserFilter : IEndpointFilter
{
    private readonly IResult _resultIfUserIsAuthenticated;
    public AuthorizedUserFilter(IResult resultIfUserIsAuthenticated)
    {
        _resultIfUserIsAuthenticated = resultIfUserIsAuthenticated;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var isAuthenticated = context.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        if (isAuthenticated)
        {
            return _resultIfUserIsAuthenticated;
        }

        return await next(context);
    }
}
