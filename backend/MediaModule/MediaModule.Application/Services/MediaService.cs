using MediaModule.Application.Dtos.Requests;
using MediaModule.Application.Dtos.Responses;
using MediaModule.Application.Interfaces;
using MediaModule.Domain.Entities;
using MediaModule.Domain.Enums;
using MediaModule.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
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

        public async Task<IEnumerable<string>> GetAllEntityMediaUrls(Guid entityId)
        {
            var medias = await _mediaRepository.GetMediasByEntityIdAsync(entityId);
            if (medias == null || !medias.Any())
                throw new Exception($"No media found for entity with Id: {entityId}");

            var response = medias.Select(m => m.Url);

            return response;
        }

        public async Task<string> GetMainMediaUrl(Guid entityId)
        {
            var media = await _mediaRepository.GetMainMediaByEntityIdAsync(entityId);
            if (media == null)
                throw new Exception($"No main media found for entity with Id: {entityId}");
            if (media.IsDeleted)
                throw new Exception($"Main media for entity with Id: {entityId} is deleted");
            if (string.IsNullOrWhiteSpace(media.Url))
                throw new Exception($"Main media URL for entity with Id: {entityId} is empty");

            return media.Url;
        }

        public async Task AddMedia(UploadMediaRequest media)
        {
            if (media == null)
                throw new ArgumentException("Media cannot be null");
            var mediaType = media.File.ContentType;
            string fileName = GenerateMediaFileName(media.EntityId, mediaType.Split('/').Last());
            using var stream = media.File.OpenReadStream();
            var url = await _mediaProvider.AddMediaAsync(fileName, mediaType, stream);
            var mediaName = fileName.Split('/').Last();
            var mediaEntity = Media.Create(media.EntityId, url, mediaName, mediaType, media.EntityType, media.IsMain);
            await _mediaRepository.AddMediaAsync(mediaEntity);

            await _mediaUnitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteMedia(Guid mediaId)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(mediaId);
            if (media == null)
                throw new Exception($"Media with Id: {mediaId} not found");
            media.MarkAsDeleted();
            await _mediaUnitOfWork.SaveChangesAsync();
        }

        public async Task HardDeleteMedia(Guid mediaId)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(mediaId);
            if (media == null)
                throw new Exception($"Media with Id: {mediaId} not found");
            var mediaFileName = CombineFileName(media.EntityId.ToString(), media.Name);
            await _mediaProvider.DeleteMediaAsync(mediaFileName);
            await _mediaRepository.DeleteMediaAsync(mediaId);
            await _mediaUnitOfWork.SaveChangesAsync();
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
