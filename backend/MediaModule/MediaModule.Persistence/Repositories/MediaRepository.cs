using MediaModule.Domain.Interfaces;
using MediaModule.Domain.Entities;
using MediaModule.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MediaModule.Persistence.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly MediaDbContext _dbContext;
        public MediaRepository(MediaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddMediaAsync(Media media, CancellationToken cancellationToken)
        {
            var exist = await _dbContext.Medias.AnyAsync(m => m.Url == media.Url, cancellationToken);
            if (exist)
                throw new MediaAlreadyExistException($"Media with Url:{media.Url} already exist");
            await _dbContext.Medias.AddAsync(media, cancellationToken);
        }

        public async Task DeleteMediaAsync(Guid mediaId, CancellationToken cancellationToken)
        {
            var deleteResult = await _dbContext.Medias
                .Where(m => m.Id == mediaId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<Dictionary<Guid, string>> GetMainMediaUrlByEntityIdsAsync(IEnumerable<Guid> entityIds, CancellationToken cancellationToken)
        {
            return await _dbContext.Medias
                .Where(m => !m.IsDeleted && m.IsMain && entityIds.Contains(m.EntityId))
                .ToDictionaryAsync(
                    m => m.EntityId,
                    m => m.Url,
                    cancellationToken);
        }

        public async Task<Media?> GetMainMediaByEntityIdAsync(Guid entityId, CancellationToken cancellationToken)
        {
            return await _dbContext.Medias
                .FirstOrDefaultAsync(m => m.EntityId == entityId && m.IsMain && !m.IsDeleted, cancellationToken);
        }

        public async Task<Media> GetMediaByIdAsync(Guid mediaId, CancellationToken cancellationToken)
        {
            var media = await _dbContext.Medias.FirstOrDefaultAsync(m => m.Id == mediaId, cancellationToken);
            if (media == null)
                throw new MediaNotFoundException($"Media with Id:{mediaId} not found");
            return media;
        }

        public async Task<IEnumerable<Media>> GetMediasByEntityIdAsync(Guid entityId, CancellationToken cancellationToken)
        {
            var medias = await _dbContext.Medias
                .Where(m => m.EntityId == entityId && !m.IsDeleted)
                .ToListAsync(cancellationToken);
            return medias;
        }
    }
}
