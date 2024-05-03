using RabbitMQ.Client;

namespace MinimalApiTemplate.REST.MessageBroker;

public sealed class RabbitMQPersistentConnection : IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection _connection;

    public RabbitMQPersistentConnection(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _ = GetConnection();
    }

    public IConnection GetConnection()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            _connection = _connectionFactory.CreateConnection();
        }

        return _connection;
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
    }
}
