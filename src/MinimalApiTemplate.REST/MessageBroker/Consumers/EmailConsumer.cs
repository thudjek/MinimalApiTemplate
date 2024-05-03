using MinimalApiTemplate.REST.Common.Interfaces;
using MinimalApiTemplate.REST.MessageBroker.Interfaces;
using MinimalApiTemplate.REST.MessageBroker.Messages;

namespace MinimalApiTemplate.REST.MessageBroker.Consumers;

public class EmailConsumer : IQueueConsumer<EmailMessage>
{
    private readonly IEmailService _emailService;
    public EmailConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task ConsumeMessage(EmailMessage message)
    {
        await _emailService.SendEmail(message.Email, message.Purpose, message.Parameters);
    }
}
