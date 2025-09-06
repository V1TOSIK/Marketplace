using MediaModule.Application.Interfaces.Repositories;
using MediaModule.Application.Interfaces;
using MediaModule.Application.Media.Commands.DeactivateMedia;
using MediatR;
using Microsoft.Extensions.Logging;
using MediaModule.Application.Interfaces.Services;
using MediaModule.Domain.Entities;
using MediaModule.Domain.Exceptions;

namespace MediaModule.Application.Media.Commands.DeleteMedia
{
    public class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand>
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly IMediaUnitOfWork _unitOfWork;
        private readonly IMediaProvider _mediaProvider;
        private readonly ILogger<DeleteMediaCommandHandler> _logger;
        public DeleteMediaCommandHandler(IMediaRepository mediaRepository,
            IMediaUnitOfWork unitOfWork,
            IMediaProvider mediaProvider,
            ILogger<DeleteMediaCommandHandler> logger)
        {
            _mediaRepository = mediaRepository;
            _unitOfWork = unitOfWork;
            _mediaProvider = mediaProvider;
            _logger = logger;
        }

        public async Task Handle(DeleteMediaCommand command, CancellationToken cancellationToken)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(command.MediaId, cancellationToken);
            var mediaFileName = CombineFileName(media.EntityId.ToString(), media.Name);

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _mediaProvider.DeleteMediaAsync(mediaFileName, cancellationToken);
                await _mediaRepository.DeleteMediaAsync(command.MediaId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            _logger.LogInformation("[Media Module] Media with ID {MediaId} successfully deleted", command.MediaId);
        }

        private string CombineFileName(string entityId, string fileName)
        {
            return $"{entityId}/{fileName}";
        }
    }
}
