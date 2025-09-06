using MediaModule.Application.Exceptions;
using MediaModule.Application.Interfaces;
using MediaModule.Application.Interfaces.Repositories;
using MediaModule.Application.Interfaces.Services;
using MediaModule.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediaModule.Application.Media.Commands.UploadMedia
{
    public class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand>
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly IMediaUnitOfWork _UnitOfWork;
        private readonly IMediaProvider _mediaProvider;
        private readonly ILogger<UploadMediaCommandHandler> _logger;
        public UploadMediaCommandHandler(IMediaRepository mediaRepository,
            IMediaUnitOfWork mediaUnitOfWork,
            IMediaProvider mediaProvider,
            ILogger<UploadMediaCommandHandler> logger)
        {
            _mediaRepository = mediaRepository;
            _UnitOfWork = mediaUnitOfWork;
            _mediaProvider = mediaProvider;
            _logger = logger;
        }

        public async Task Handle(UploadMediaCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
            {
                _logger.LogWarning("[Media Module] UploadMediaCommand is null.");
                throw new NullableMediaException("Media cannot be null");
            }

            var mediaType = command.File.ContentType;
            string fileName = GenerateMediaFileName(command.EntityId, mediaType.Split('/').Last());
            using var stream = command.File.OpenReadStream();
            var url = await _mediaProvider.AddMediaAsync(fileName, mediaType, stream, cancellationToken);
            var mediaName = fileName.Split('/').Last();
            var mediaEntity = Domain.Entities.Media.Create(command.EntityId, url, mediaName, mediaType, command.EntityType, command.IsMain);
            await _mediaRepository.AddMediaAsync(mediaEntity, cancellationToken);

            await _UnitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Media Module] Media '{MediaName}' (ID: {MediaId}) successfully uploaded for Entity ID: {EntityId}", mediaName, mediaEntity.Id, command.EntityId);
        }

        private string GenerateMediaFileName(Guid entityId, string extentions)
        {
            return $"{entityId}/{Guid.NewGuid()}.{extentions}";
        }
    }
}
