using MinimalApiTemplate.REST.Enums;

namespace MinimalApiTemplate.REST.Services.Email;

public class SendGridConfig
{
    public const string SectionName = "SendGrid";

    public string ApiKey { get; set; }
    public string FromEmail { get; set; }
    public string FromDisplayName { get; set; }
    public List<EmailType> EmailTypes { get; set; }
}

public class EmailType
{
    public EmailPurpose Purpose { get; set; }
    public string Subject { get; set; }
    public string TemplateId { get; set; }
}