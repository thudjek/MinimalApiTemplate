
using System.Diagnostics;

namespace MinimalApiTemplate.REST.Common.Filters;

public class RequestLoggingFilter : IEndpointFilter
{
    private readonly ILogger<RequestLoggingFilter> _logger;
    public RequestLoggingFilter(ILogger<RequestLoggingFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        _logger.LogInformation("Request HTTP {Method} {Path} started", context.HttpContext.Request.Method, context.HttpContext.Request.Path);

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var response = await next(context);

        stopwatch.Stop();

        _logger.LogInformation("Request HTTP {Method} {Path} finished in {TimeElapsed} miliseconds", context.HttpContext.Request.Method, context.HttpContext.Request.Path, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
