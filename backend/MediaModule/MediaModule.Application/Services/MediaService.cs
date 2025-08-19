using MediaModule.Application.Dtos.Requests;
using MediaModule.Application.Dtos.Responses;
using MediaModule.Application.Exceptions;
using MediaModule.Application.Interfaces;
using MediaModule.Domain.Entities;
using MediaModule.Domain.Exceptions;
using MediaModule.Domain.Interfaces;
using SharedKernel.Interfaces;

namespace MediaModule.Application.Services
{
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly IMediaProvider _mediaProvider;
        private readonly IMediaUnitOfWork _mediaUnitOfWork;
        public MediaService(IMediaRepository mediaRepository,
            IMediaProvider mediaProvider,
            IMediaUnitOfWork mediaUnitOfWork)
        {
            _mediaRepository = mediaRepository;
            _mediaProvider = mediaProvider;
            _mediaUnitOfWork = mediaUnitOfWork;
        }

        public async Task<IEnumerable<MediaResponse>> GetAllEntityMediaUrls(Guid entityId, CancellationToken cancellationToken)
        {
            var medias = await _mediaRepository.GetMediasByEntityIdAsync(entityId, cancellationToken);

            var response = medias.Select(m => new MediaResponse
            {
                Url = m.Url,
            });

            return response;
        }

        public async Task AddMedia(UploadMediaRequest media, CancellationToken cancellationToken)
        {
            if (media == null)
                throw new NullableMediaException("Media cannot be null");

            var mediaType = media.File.ContentType;
            string fileName = GenerateMediaFileName(media.EntityId, mediaType.Split('/').Last());
            using var stream = media.File.OpenReadStream();
            var url = await _mediaProvider.AddMediaAsync(fileName, mediaType, stream, cancellationToken);
            var mediaName = fileName.Split('/').Last();
            var mediaEntity = Media.Create(media.EntityId, url, mediaName, mediaType, media.EntityType, media.IsMain);
            await _mediaRepository.AddMediaAsync(mediaEntity, cancellationToken);

            await _mediaUnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task SoftDeleteMedia(Guid mediaId, CancellationToken cancellationToken)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(mediaId, cancellationToken);
            if (media == null)
                throw new MediaNotFoundException($"Media with Id: {mediaId} not found");
            media.MarkAsDeleted();
            await _mediaUnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task HardDeleteMedia(Guid mediaId, CancellationToken cancellationToken)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(mediaId, cancellationToken);
            if (media == null)
                throw new MediaNotFoundException($"Media with Id: {mediaId} not found");
            var mediaFileName = CombineFileName(media.EntityId.ToString(), media.Name);
            await _mediaProvider.DeleteMediaAsync(mediaFileName, cancellationToken);
            await _mediaRepository.DeleteMediaAsync(mediaId, cancellationToken);
            await _mediaUnitOfWork.SaveChangesAsync(cancellationToken);
        }

        private string GenerateMediaFileName(Guid entityId, string extentions)
        {
                return $"{entityId}/{Guid.NewGuid()}.{extentions}";
        }

        private string CombineFileName(string entityId, string fileName)
        {
            return $"{entityId}/{fileName}";
        }
    }
}
