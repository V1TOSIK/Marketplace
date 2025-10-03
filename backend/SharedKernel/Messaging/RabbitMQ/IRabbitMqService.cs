namespace SharedKernel.Messaging.RabbitMQ
{
    public interface IRabbitMqService
    {
        Task PublishAsync(string message, string exchangeName, string routingKey, CancellationToken cancellationToken = default);
        Task DeclareExchangeAsync(string exchangeName, string type = "direct", CancellationToken cancellationToken = default);
        Task DeclareQueueAsync(string queueName, IDictionary<string, object>? arguments = null, CancellationToken cancellationToken = default);
        Task BindQueueAsync(string queueName, string exchangeName, string routingKey, CancellationToken cancellationToken = default);
        Task ListenQueueAsync(string queueName, Func<string, Task> onMessageReceived, CancellationToken cancellationToken = default);
    }
}
