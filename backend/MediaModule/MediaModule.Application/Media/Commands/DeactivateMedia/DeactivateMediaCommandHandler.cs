using MediaModule.Application.Interfaces.Repositories;
using MediaModule.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediaModule.Application.Media.Commands.DeactivateMedia
{
    public class DeactivateMediaCommandHandler : IRequestHandler<DeactivateMediaCommand>
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly IMediaUnitOfWork _unitOfWork;
        private readonly ILogger<DeactivateMediaCommandHandler> _logger;
        public DeactivateMediaCommandHandler(IMediaRepository mediaRepository,
            IMediaUnitOfWork unitOfWork,
            ILogger<DeactivateMediaCommandHandler> logger)
        {
            _mediaRepository = mediaRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeactivateMediaCommand command, CancellationToken cancellationToken)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(command.MediaId, cancellationToken);
            media.MarkAsDeleted();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Media Module] Media with ID {MediaId} has been soft deleted.", command.MediaId);
        }
    }
}
