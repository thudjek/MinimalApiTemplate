namespace MinimalApiTemplate.REST.MessageBroker.Interfaces;

public interface IQueueConsumerHandler<TMessageConsumer, TMessage>
    where TMessageConsumer : IQueueConsumer<TMessage>
    where TMessage : IQueueMessage
{
    Task InitializeConsumer();
    void CancelConsumer();
}
