using MediaModule.Domain.Entities;
using MediaModule.Domain.Enums;

namespace MediaModule.Domain.Interfaces
{
    public interface IMediaRepository
    {
        Task<Media> GetMediaByIdAsync(Guid mediaId);
        Task<IEnumerable<Media>> GetAllMediaAsync();
        Task<Dictionary<Guid, string>> GetMainMediaUrlByEntityIdsAsync(IEnumerable<Guid> entityIds);
        Task AddMediaAsync(Media media);
        Task DeleteMediaAsync(Guid mediaId);
        Task<Media?> GetMainMediaByEntityIdAsync(Guid entityId);
        Task<IEnumerable<Media>> GetMediasByEntityIdAsync(Guid userId);
        Task<IEnumerable<Media>> GetMediaByTypeAsync(string type);
    }
}
