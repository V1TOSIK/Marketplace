using MediaModule.Application.Exceptions;
using MediaModule.Domain.Interfaces;
using SharedKernel.Interfaces;

namespace MediaModule.Application.Services
{
    public class MediaManager : IMediaManager
    {
        private readonly IMediaRepository _mediaRepository;
        public MediaManager(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }
        public async Task<IEnumerable<string>> GetAllEntityMediaUrls(Guid entityId, CancellationToken cancellationToken)
        {
            var medias = await _mediaRepository.GetMediasByEntityIdAsync(entityId, cancellationToken);

            return medias.Select(m => m.Url);
        }


        public async Task<Dictionary<Guid, string>> GetAllMainMediaUrls(IEnumerable<Guid> entityIds, CancellationToken cancellationToken)
        {
            return await _mediaRepository
                .GetMainMediaUrlByEntityIdsAsync(entityIds, cancellationToken);
        }

        public async Task<string> GetMainMediaUrl(Guid entityId, CancellationToken cancellationToken)
        {
            var media = await _mediaRepository.GetMainMediaByEntityIdAsync(entityId, cancellationToken);
            if (media == null)
                throw new NullableMediaException("Media cannot be null");

            return media.Url;
        }
    }
}
