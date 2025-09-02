using MediaModule.Domain.Entities;

namespace MediaModule.Application.Interfaces.Repositories
{
    public interface IMediaRepository
    {
        Task<Media> GetMediaByIdAsync(Guid mediaId, CancellationToken cancellationToken);
        Task<Dictionary<Guid, string>> GetMainMediaUrlByEntityIdsAsync(IEnumerable<Guid> entityIds, CancellationToken cancellationToken);
        Task AddMediaAsync(Media media, CancellationToken cancellationToken);
        Task DeleteMediaAsync(Guid mediaId, CancellationToken cancellationToken);
        Task<Media?> GetMainMediaByEntityIdAsync(Guid entityId, CancellationToken cancellationToken);
        Task<IEnumerable<Media>> GetMediasByEntityIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
