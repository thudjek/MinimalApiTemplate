using MinimalApiTemplate.REST.Enums;
using MinimalApiTemplate.REST.MessageBroker.Interfaces;

namespace MinimalApiTemplate.REST.MessageBroker.Messages;

public class EmailMessage : IQueueMessage
{
    public string Email { get; set; }
    public EmailPurpose Purpose { get; set; }
    public object Parameters { get; set; }
}
