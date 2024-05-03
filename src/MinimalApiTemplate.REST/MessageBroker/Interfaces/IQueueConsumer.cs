namespace MinimalApiTemplate.REST.MessageBroker.Interfaces;

public interface IQueueConsumer<in TMessage> where TMessage : IQueueMessage
{
    Task ConsumeMessage(TMessage message);
}
