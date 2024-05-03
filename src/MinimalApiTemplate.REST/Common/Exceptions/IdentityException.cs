namespace MinimalApiTemplate.REST.Common.Exceptions;

public class IdentityException : Exception, ICustomException
{
    public IdentityException() : base("IdentityException has occurred")
    {
    }

    public IdentityException(string message) : base(message)
    {
    }

    public IdentityException(IEnumerable<string> errors) : base("IdentityException has occurred")
    {
        Errors = errors;
    }

    public IdentityException(string message, IEnumerable<string> errors) : base(message)
    {
        Errors = errors;
    }

    public IEnumerable<string> Errors { get; }

    public void LogException(ILogger logger)
    {
        if (Errors != null && Errors.Any())
        {
            logger.LogError(this, "IdentityException has occurred. Errors: {@Errors}", Errors);
        }
        else
        {
            logger.LogError(this, "IdentityException has occurred.");
        }
    }
}
