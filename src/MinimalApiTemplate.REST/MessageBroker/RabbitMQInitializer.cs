using RabbitMQ.Client;

namespace MinimalApiTemplate.REST.MessageBroker;

public class RabbitMQInitializer
{
    private readonly RabbitMQConfig _rabbitMqConfig;
    private readonly RabbitMQPersistentConnection _persistentConnection;
    public RabbitMQInitializer(RabbitMQConfig rabbitMqConfig, RabbitMQPersistentConnection persistentConnection)
    {
        _rabbitMqConfig = rabbitMqConfig;
        _persistentConnection = persistentConnection;
    }

    public void InitializeExchangesAndQueues()
    {
        var connection = _persistentConnection.GetConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(_rabbitMqConfig.ExchangeName, ExchangeType.Direct);

        foreach (var queue in _rabbitMqConfig.Queues)
        {
            var deadLetterExchangeName = $"{queue.Name}DLX";
            var deadLetterQueueName = $"{queue.Name}DeadLetter";

            var deadLetterQueueArgs = new Dictionary<string, object>
            {
                { "x-queue-type", "quorum" },
                { "overflow", "reject-publish" }
            };

            channel.ExchangeDeclare(deadLetterExchangeName, ExchangeType.Direct);
            channel.QueueDeclare(deadLetterQueueName, true, false, false, null);
            channel.QueueBind(deadLetterQueueName, deadLetterExchangeName, deadLetterQueueName, null); ;

            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", deadLetterExchangeName },
                { "x-dead-letter-routing-key", deadLetterQueueName },
                { "x-queue-type", "quorum" },
                { "x-dead-letter-strategy", "at-least-once" },
                { "overflow", "reject-publish" }
            };

            channel.QueueDeclare(queue.Name, true, false, false, queueArgs);
            channel.QueueBind(queue.Name, _rabbitMqConfig.ExchangeName, queue.Name, null);
        }

        channel.Close();
    }
}
