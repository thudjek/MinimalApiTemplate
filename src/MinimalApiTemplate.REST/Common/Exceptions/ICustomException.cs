namespace MinimalApiTemplate.REST.Common.Exceptions;

public interface ICustomException
{
    void LogException(ILogger logger);
}
