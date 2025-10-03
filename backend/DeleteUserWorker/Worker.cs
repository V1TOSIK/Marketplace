using AuthModule.Application.Interfaces.Repositories;
using SharedKernel.Messaging.RabbitMQ;

namespace DeleteUserWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IRabbitMqService _rabbitMqService;

        public Worker(ILogger<Worker> logger,
            IAuthUserRepository authUserRepository,
            IRabbitMqService rabbitMqService)
        {
            _logger = logger;
            _authUserRepository = authUserRepository;
            _rabbitMqService = rabbitMqService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitMqService.DeclareExchangeAsync("user_exchange", cancellationToken: stoppingToken);
            await _rabbitMqService.DeclareQueueAsync("user_delete_queue", cancellationToken: stoppingToken);
            await _rabbitMqService.BindQueueAsync("user_delete_queue", "user_exchange", "user.delete", stoppingToken);
            await _rabbitMqService.ListenQueueAsync("user_delete_queue", async (message) =>
            {
                if (Guid.TryParse(message, out var userId))
                {
                    _logger.LogInformation($"Received request to delete user with ID: {userId}");
                    await _authUserRepository.HardDeleteAsync(userId, stoppingToken);
                    _logger.LogInformation($"User with ID: {userId} has been deleted.");
                }
                else
                {
                    _logger.LogWarning($"Invalid user ID received: {message}");
                }
            }, stoppingToken);

        }
    }
}
