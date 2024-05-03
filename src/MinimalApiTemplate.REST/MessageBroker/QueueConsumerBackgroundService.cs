using MinimalApiTemplate.REST.MessageBroker.Interfaces;

namespace MinimalApiTemplate.REST.MessageBroker;

public class QueueConsumerBackgroundService<TMessageConsumer, TMessage> : BackgroundService
    where TMessageConsumer : IQueueConsumer<TMessage>
    where TMessage : IQueueMessage
{
    private IQueueConsumerHandler<TMessageConsumer, TMessage> consumerHandler;
    private IServiceScope scope;

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueueConsumerBackgroundService<TMessageConsumer, TMessage>> _logger;

    public QueueConsumerBackgroundService(IServiceProvider serviceProvider, ILogger<QueueConsumerBackgroundService<TMessageConsumer, TMessage>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        scope = _serviceProvider.CreateScope();
        consumerHandler = scope.ServiceProvider.GetRequiredService<IQueueConsumerHandler<TMessageConsumer, TMessage>>();

        _logger.LogInformation("Initializing message consumers started");
        consumerHandler.InitializeConsumer();
        _logger.LogInformation("Initializing message consumers finished");

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Canceling message consumers started");
        consumerHandler.CancelConsumer();
        _logger.LogInformation("Canceling message consumers finished");

        return base.StopAsync(cancellationToken);
    }
}
