using MediaModule.Application.Interfaces.Repositories;
using MediaModule.Application.Interfaces.Services;
using SharedKernel.Messaging.RabbitMQ;
using System.Text.Json;

namespace DeleteMediaWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMediaRepository _mediaRepository;
        private readonly IMediaProvider _mediaProvider;
        private readonly IRabbitMqService _rabbitMqService;

        public Worker(ILogger<Worker> logger,
            IMediaRepository mediaRepository,
            IMediaProvider mediaProvider,
            IRabbitMqService rabbitMqService)
        {
            _logger = logger;
            _mediaRepository = mediaRepository;
            _mediaProvider = mediaProvider;
            _rabbitMqService = rabbitMqService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _rabbitMqService.DeclareExchangeAsync("product_exchange", cancellationToken: cancellationToken);
            await _rabbitMqService.DeclareExchangeAsync("user_exchange", cancellationToken: cancellationToken);
            await _rabbitMqService.DeclareQueueAsync("media_delete_queue", cancellationToken: cancellationToken);
            await _rabbitMqService.BindQueueAsync("media_delete_queue", "product_exchange", "product.delete", cancellationToken);
            await _rabbitMqService.BindQueueAsync("media_delete_queue", "user_exchange", "user.delete", cancellationToken);
            await _rabbitMqService.ListenQueueAsync("media_delete_queue", async (message) =>
            {
                var entityIds = JsonSerializer.Deserialize<List<Guid>>(message);
                if (entityIds == null)
                {
                    _logger.LogWarning("[DeleteMediaWorker] Invalid entity IDs received: {message}", message);
                    return;
                }
                if (entityIds.Count == 0)
                {
                    _logger.LogWarning("[DeleteMediaWorker] Empty entity IDs list received.");
                    return;
                }

                foreach (var entityId in entityIds)
                {
                    var mediaList = await _mediaRepository.GetMediasByEntityIdAsync(entityId, cancellationToken);
                    _logger.LogInformation("[DeleteMediaWorker] Received request to delete media with entity ID: {entityd}", entityId);
                    foreach (var media in mediaList)
                    {
                        try
                        {
                            await _mediaProvider.DeleteMediaAsync(media.Name, cancellationToken);
                            await _mediaRepository.DeleteMediaAsync(media.Id, cancellationToken);
                            _logger.LogInformation("[DeleteMediaWorker] Deleted media with ID: {mediaId}", media.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "[DeleteMediaWorker] Error deleting media with ID: {mediaId}", media.Id);
                        }
                    }
                    _logger.LogInformation("[DeleteMediaWorker] Media with Entity ID: {entityId} has been processed for deletion.", entityId);
                }
            }, cancellationToken);
        }
    }
}
