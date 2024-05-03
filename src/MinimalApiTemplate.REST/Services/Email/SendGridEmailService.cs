using System.Text.Json;
using MinimalApiTemplate.REST.Common.Interfaces;
using MinimalApiTemplate.REST.Enums;

namespace MinimalApiTemplate.REST.Services.Email;

public class SendGridEmailService : IEmailService
{
    //private readonly ISendGridClient _sendGridClient;
    private readonly SendGridConfig _sendGridConfig;
    private readonly ILogger<SendGridEmailService> _logger;
    public SendGridEmailService(/*ISendGridClient sendGridClient, */SendGridConfig sendGridConfig, ILogger<SendGridEmailService> logger)
    {
        //_sendGridClient = sendGridClient;
        _sendGridConfig = sendGridConfig;
        _logger = logger;
    }

    public async Task SendEmail(string toEmail, EmailPurpose purpose, object parameters = null)
    {
        //var sendGridMessage = new SendGridMessage();

        //var emailType = _sendGridConfig.EmailTypes.FirstOrDefault(et => et.Purpose == purpose);

        //if (emailType == null)
        //{
        //    throw new EmailPurposeException(purpose);
        //}

        //sendGridMessage.SetFrom(_sendGridConfig.FromEmail, _sendGridConfig.FromDisplayName);
        //sendGridMessage.AddTo(toEmail);
        //sendGridMessage.SetSubject(emailType.Subject);
        //sendGridMessage.SetTemplateId(emailType.TemplateId);

        //if (parameters != null)
        //{
        //    sendGridMessage.SetTemplateData(parameters);
        //}

        //var emailResponse = await _sendGridClient.SendEmailAsync(sendGridMessage);

        _logger.LogInformation("Simulating sending email to {ToEmail}. {Parameters}", toEmail, JsonSerializer.Serialize(parameters));
        await Task.CompletedTask;
    }
}
