using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using MinimalApiTemplate.REST.MessageBroker.Interfaces;

namespace MinimalApiTemplate.REST.MessageBroker;

public class RabbitMQPublisher : IQueuePublisher
{
    private readonly RabbitMQPersistentConnection _persistedConnection;
    private readonly IConfiguration _configuration;
    private readonly RabbitMQConfig _rabbitMQConfig;
    public RabbitMQPublisher(RabbitMQPersistentConnection persistedConnection, IConfiguration configuration, RabbitMQConfig rabbitMQConfig)
    {
        _persistedConnection = persistedConnection;
        _configuration = configuration;
        _rabbitMQConfig = rabbitMQConfig;
    }
    public void PublishMessage<TMessage>(TMessage message)
        where TMessage : IQueueMessage
    {
        var messageType = typeof(TMessage).Name;
        var queueName = _rabbitMQConfig.Queues?.FirstOrDefault(q => q.MessageType == messageType)?.Name;

        if (queueName == null)
        {
            throw new QueueException($"Failed to publish message. Queue for message type {messageType} not found in config");
        }

        var connection = _persistedConnection.GetConnection();
        var channel = connection.CreateModel();

        var messageAsJson = JsonSerializer.Serialize(message);
        var messageBytes = Encoding.UTF8.GetBytes(messageAsJson);

        try
        {
            channel.BasicPublish(_configuration["RabbitMQ:ExchangeName"], queueName, null, messageBytes);
        }
        catch (Exception ex)
        {
            throw new QueueException($"Failed to publish message.", ex);
        }

        channel.Close();
    }
}
