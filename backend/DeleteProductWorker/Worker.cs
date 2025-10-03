using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Messaging.RabbitMQ;
using System.Text.Json;

namespace DeleteProductWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IRabbitMqService _rabbitMqService;

        public Worker(ILogger<Worker> logger,
            IProductRepository productRepository,
            IRabbitMqService rabbitMqService)
        {
            _logger = logger;
            _productRepository = productRepository;
            _rabbitMqService = rabbitMqService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _rabbitMqService.DeclareExchangeAsync("user_exchange", "direct", cancellationToken);
            await _rabbitMqService.DeclareQueueAsync("product_delete_queue", cancellationToken: cancellationToken);
            await _rabbitMqService.BindQueueAsync("product_delete_queue", "user_exchange", "user.delete", cancellationToken);

            await _rabbitMqService.ListenQueueAsync("product_delete_queue", async (message) =>
            {
                if (Guid.TryParse(message, out var userId))
                {
                    var productIds = await _productRepository.GetProductIdsByUserIdAsync(userId, cancellationToken);
                    _logger.LogInformation("[DeleteProductWorker] Received request to delete products with user ID: {userId}", userId);

                    var batchSize = 100;
                    foreach (var productId in productIds.Chunk(batchSize))
                    {
                        var body = JsonSerializer.Serialize(productId);
                        await _rabbitMqService.PublishAsync(body, "product_exchange", "product.delete", cancellationToken);
                        _logger.LogInformation("[DeleteProductWorker] Deleting product with ID: {productId}", productId);
                    }

                    await _productRepository.DeleteProductsByIdsAsync(productIds, cancellationToken);
                    _logger.LogInformation("[DeleteProductWorker] Product with User ID: {userId} has been deleted.", userId);
                }
                else
                {
                    _logger.LogWarning("[DeleteProductWorker] Invalid user ID received: {message}", message);
                }
            }, cancellationToken);
        }
    }
}
