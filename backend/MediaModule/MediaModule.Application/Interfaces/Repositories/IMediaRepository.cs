using MediaModule.Domain.Entities;
using SharedKernel.Dtos;

namespace MediaModule.Application.Interfaces.Repositories
{
    public interface IMediaRepository
    {
        Task<Domain.Entities.Media> GetMediaByIdAsync(Guid mediaId, CancellationToken cancellationToken);
        Task<Dictionary<Guid, MediaDto>> GetMainMediaByEntityIdsAsync(List<Guid> entityIds, CancellationToken cancellationToken);
        Task AddMediaAsync(Domain.Entities.Media media, CancellationToken cancellationToken);
        Task DeleteMediaAsync(Guid mediaId, CancellationToken cancellationToken);
        Task DeleteEntityMediasAsync(Guid entityId, CancellationToken cancellationToken);
        Task<Domain.Entities.Media> GetMainMediaByEntityIdAsync(Guid entityId, CancellationToken cancellationToken);
        Task<IEnumerable<Domain.Entities.Media>> GetMediasByEntityIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
