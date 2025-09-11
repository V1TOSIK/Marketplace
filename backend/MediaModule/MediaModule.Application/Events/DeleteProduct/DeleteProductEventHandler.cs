using MediaModule.Application.Interfaces;
using MediaModule.Application.Interfaces.Repositories;
using MediaModule.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;

namespace MediaModule.Application.Events.DeleteProduct
{
    public class DeleteProductEventHandler : INotificationHandler<DeleteProductDomainEvent>
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly IMediaService _mediaService;
        private readonly IMediaProvider _mediaProvider;
        private readonly IMediaUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteProductEventHandler> _logger;
        public DeleteProductEventHandler(IMediaRepository mediaRepository,
            IMediaService mediaService,
            IMediaProvider mediaProvider,
            IMediaUnitOfWork unitOfWork,
            ILogger<DeleteProductEventHandler> logger)
        {
            _mediaRepository = mediaRepository;
            _mediaService = mediaService;
            _mediaProvider = mediaProvider;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeleteProductDomainEvent notification, CancellationToken cancellationToken)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var medias = await _mediaRepository.GetMediasByEntityIdAsync(notification.ProductId, cancellationToken);
                foreach (var media in medias)
                {
                    var name = _mediaService.CombineFileName(notification.ProductId.ToString(), media.Name);
                    await _mediaProvider.DeleteMediaAsync(name, cancellationToken);
                }
                await _mediaRepository.DeleteEntityMediasAsync(notification.ProductId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            _logger.LogInformation("[Media Module (Event)] All medias of product with Id:{ProductId} have been deleted.", notification.ProductId);
        }
    }
}
