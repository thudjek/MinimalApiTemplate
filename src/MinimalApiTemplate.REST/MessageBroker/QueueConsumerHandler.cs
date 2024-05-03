using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using MinimalApiTemplate.REST.Common.Exceptions;
using MinimalApiTemplate.REST.MessageBroker.Interfaces;

namespace MinimalApiTemplate.REST.MessageBroker;

public class QueueConsumerHandler<TMessageConsumer, TMessage> : IQueueConsumerHandler<TMessageConsumer, TMessage>
    where TMessageConsumer : IQueueConsumer<TMessage>
    where TMessage : IQueueMessage
{
    private string consumerTag;
    private AsyncEventingBasicConsumer consumer;
    private readonly IConnection _connection;
    private readonly RabbitMQConfig _rabbitMQConfig;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueueConsumerHandler<TMessageConsumer, TMessage>> _logger;
    public QueueConsumerHandler(RabbitMQPersistentConnection persistentConnection, RabbitMQConfig rabbitMQConfig, IServiceProvider serviceProvider, ILogger<QueueConsumerHandler<TMessageConsumer, TMessage>> logger)
    {
        _connection = persistentConnection.GetConnection();
        _rabbitMQConfig = rabbitMQConfig;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task InitializeConsumer()
    {
        var messageType = typeof(TMessage).Name;
        var queueName = _rabbitMQConfig.Queues?.FirstOrDefault(q => q.MessageType == messageType)?.Name;

        if (queueName == null)
            throw new QueueException($"Queue for message type {messageType} not found in config.");

        var channel = _connection.CreateModel();
        channel.BasicQos(0, 1, false);

        consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += HandleMessage;

        try
        {
            consumerTag = channel.BasicConsume(queueName, false, consumer);
        }
        catch (Exception ex)
        {
            throw new QueueException($"BasicConsume for consumer {nameof(TMessageConsumer)} failed.", ex);
        }

        await Task.CompletedTask;
    }

    public void CancelConsumer()
    {
        try
        {
            consumer.Model.BasicCancel(consumerTag);
        }
        catch (Exception ex)
        {
            throw new QueueException($"BasicCancel for consumer {nameof(TMessageConsumer)} failed.", ex);
        }
    }

    private async Task HandleMessage(object ch, BasicDeliverEventArgs ea)
    {
        var channel = ((AsyncEventingBasicConsumer)ch).Model;

        try
        {
            var consumerInstance = _serviceProvider.GetRequiredService<TMessageConsumer>();

            var messageAsJson = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<TMessage>(messageAsJson);
            await consumerInstance.ConsumeMessage(message);

            channel.BasicAck(ea.DeliveryTag, false);
        }
        catch (JsonException)
        {
            _logger.LogError("Error while trying to deserialize message of type {MessageType}", typeof(TMessage));
            channel.BasicReject(ea.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            var exceptionType = ex.GetType();

            if (typeof(ICustomException).IsAssignableFrom(exceptionType))
            {
                var iex = ex as ICustomException;
                iex.LogException(_logger);
            }
            else
            {
                _logger.LogError(ex, "Unhandeled exception of type {ExceptionType} occurred while trying to consume message of type {MessageType}.", exceptionType.Name, typeof(TMessage));
            }

            channel.BasicReject(ea.DeliveryTag, false);
        }
    }
}
