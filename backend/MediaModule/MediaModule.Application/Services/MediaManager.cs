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
        public async Task<IEnumerable<string>> GetAllEntityMediaUrls(Guid entityId)
        {
            var medias = await _mediaRepository.GetMediasByEntityIdAsync(entityId);

            return medias.Select(m => m.Url);
        }


        public async Task<Dictionary<Guid, string>> GetAllMainMediaUrls(IEnumerable<Guid> entityIds)
        {
            var mediaUrls = await _mediaRepository.GetMainMediaUrlByEntityIdsAsync(entityIds);

            return mediaUrls;
        }

        public async Task<string> GetMainMediaUrl(Guid entityId)
        {
            var media = await _mediaRepository.GetMainMediaByEntityIdAsync(entityId);
            if (media == null)
                throw new ArgumentNullException(nameof(media));

            return media.Url;
        }
    }
}
