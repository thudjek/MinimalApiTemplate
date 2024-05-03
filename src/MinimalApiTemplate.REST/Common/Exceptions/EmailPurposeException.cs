
using MinimalApiTemplate.REST.Enums;

namespace MinimalApiTemplate.REST.Common.Exceptions;

public class EmailPurposeException : Exception, ICustomException
{
    public EmailPurposeException(EmailPurpose purpose) : base($"Email type for purpose {purpose} is not defined in configuration")
    {
        Purpose = purpose;
    }

    public EmailPurpose Purpose { get; set; }

    public void LogException(ILogger logger)
    {
        logger.LogError(this, "Email type for purpose {Purpose} is not defined in configuration", Purpose);
    }
}
