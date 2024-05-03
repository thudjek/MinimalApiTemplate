using MinimalApiTemplate.REST.MessageBroker.Interfaces;

namespace MinimalApiTemplate.REST.MessageBroker;

public static class MessageQueueExtensions
{
    public static IServiceCollection AddQueueMessageConsumer<TMessageConsumer, TMessage>(this IServiceCollection services)
        where TMessageConsumer : IQueueConsumer<TMessage>
        where TMessage : IQueueMessage
    {
        services.AddScoped(typeof(TMessageConsumer));
        services.AddScoped<IQueueConsumerHandler<TMessageConsumer, TMessage>, QueueConsumerHandler<TMessageConsumer, TMessage>>();
        services.AddHostedService<QueueConsumerBackgroundService<TMessageConsumer, TMessage>>();

        return services;
    }
}
