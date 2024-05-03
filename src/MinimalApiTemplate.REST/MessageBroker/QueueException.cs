namespace MinimalApiTemplate.REST.MessageBroker;

public class QueueException : Exception
{
    public QueueException(string message) : base(message)
    {
    }

    public QueueException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
