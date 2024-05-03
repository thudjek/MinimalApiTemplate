using MinimalApiTemplate.REST.Enums;

namespace MinimalApiTemplate.REST.Common.Interfaces;

public interface IEmailService
{
    Task SendEmail(string toEmail, EmailPurpose purpose, object parameters = null);
}
