namespace MinimalApiTemplate.REST.MessageBroker.Interfaces;

public interface IQueuePublisher
{
    void PublishMessage<TMessage>(TMessage message) where TMessage : IQueueMessage;
}
