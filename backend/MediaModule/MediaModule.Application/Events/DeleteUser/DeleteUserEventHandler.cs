using MediaModule.Application.Interfaces;
using MediaModule.Application.Interfaces.Repositories;
using MediaModule.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;

namespace MediaModule.Application.Events.DeleteUser
{
    public class DeleteUserEventHandler : INotificationHandler<HardDeleteUserEvent>
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly IMediaProvider _mediaProvider;
        private readonly IMediaService _mediaService;
        private readonly IMediaUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteUserEventHandler> _logger;
        public DeleteUserEventHandler(IMediaRepository mediaRepository,
            IMediaProvider mediaProvider,
            IMediaService mediaService,
            IMediaUnitOfWork unitOfWork,
            ILogger<DeleteUserEventHandler> logger)
        {
            _mediaRepository = mediaRepository;
            _mediaProvider = mediaProvider;
            _mediaService = mediaService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(HardDeleteUserEvent notification, CancellationToken cancellationToken)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var medias = await _mediaRepository.GetMediasByEntityIdAsync(notification.UserId, cancellationToken);
                foreach (var media in medias)
                {
                    var name = _mediaService.CombineFileName(notification.UserId.ToString(), media.Name);
                    await _mediaProvider.DeleteMediaAsync(name, cancellationToken);
                }
            
                await _mediaRepository.DeleteEntityMediasAsync(notification.UserId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            _logger.LogInformation("[Media Module (Event)] All medias of user with Id:{UserId} have been deleted.", notification.UserId);
        }
    }
}
