using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace SharedKernel.Messaging.RabbitMQ
{
    public class RabbitMqService : IRabbitMqService
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly ConnectionFactory _factory;

        public RabbitMqService(string hostName, string userName, string password)
        {
            _factory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task DeclareExchangeAsync(string exchangeName, string type = "direct", CancellationToken cancellationToken = default)
        {
            if (_channel == null)
                throw new InvalidOperationException("RabbitMQ channel is not initialized. Call InitializeAsync() before declaring exchanges.");
            await _channel.ExchangeDeclareAsync(exchangeName, type, true, false, null);
            Console.WriteLine($"[x] Declared exchange '{exchangeName}' of type '{type}'");
        }

        public async Task PublishAsync(string message, string exchangeName, string routingKey, CancellationToken cancellationToken = default)
        {
            if (_channel == null)
                throw new InvalidOperationException("RabbitMQ channel is not initialized. Call InitializeAsync() before publishing messages.");

            DeclareExchangeAsync(exchangeName, cancellationToken: cancellationToken).GetAwaiter().GetResult();

            var body = System.Text.Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchangeName, routingKey, body, cancellationToken);
            Console.WriteLine($"[x] Sent '{message}' to exchange '{exchangeName}' with routing key '{routingKey}'");
        }

        public async Task DeclareQueueAsync(string queueName, IDictionary<string, object>? arguments = null, CancellationToken cancellationToken = default)
        {
            if (_channel == null)
                throw new InvalidOperationException("RabbitMQ channel is not initialized. Call InitializeAsync() before declaring queues.");
            await _channel.QueueDeclareAsync(queueName, true, false, false, arguments);
            Console.WriteLine($"[x] Declared queue '{queueName}'");
        }

        public async Task BindQueueAsync(string queueName, string exchangeName, string routingKey, CancellationToken cancellationToken = default)
        {
            if (_channel == null)
                throw new InvalidOperationException("RabbitMQ channel is not initialized. Call InitializeAsync() before binding queues.");
            await _channel.QueueBindAsync(queueName, exchangeName, routingKey, null);
            Console.WriteLine($"[x] Bound queue '{queueName}' to exchange '{exchangeName}' with routing key '{routingKey}'");
        }

        public async Task ListenQueueAsync(string queueName, Func<string, Task> onMessageReceived, CancellationToken cancellationToken = default)
        {
            if (_channel == null)
                throw new InvalidOperationException("RabbitMQ channel is not initialized. Call InitializeAsync() before binding queues.");
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await onMessageReceived(message);
            };

            await _channel.BasicConsumeAsync(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer,
                                     cancellationToken: cancellationToken);
        }
    }
}
