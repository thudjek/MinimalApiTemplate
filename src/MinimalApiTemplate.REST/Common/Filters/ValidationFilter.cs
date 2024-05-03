
using FluentValidation;

namespace MinimalApiTemplate.REST.Common.Filters;

public class ValidationFilter<TRequest> : IEndpointFilter
{
    private readonly ILogger<ValidationFilter<TRequest>> _logger;
    private readonly IValidator<TRequest> _validator;
    public ValidationFilter(ILogger<ValidationFilter<TRequest>> logger, IValidator<TRequest> validator = null)
    {
        _logger = logger;
        _validator = validator;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var requestName = typeof(TRequest).Name;

        if (_validator == null)
        {
            _logger.LogWarning("No validators configured for {RequestName}", requestName);
            return await next(context);
        }

        var request = context.Arguments.OfType<TRequest>().First();
        var validationResult = await _validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (validationResult.IsValid)
        {
            return await next(context);
        }

        return Results.ValidationProblem(validationResult.ToDictionary(), title: "Bad Request", detail: "One or more validation errors occurred.");
    }
}
