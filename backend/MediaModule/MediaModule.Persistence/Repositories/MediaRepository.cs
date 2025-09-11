using MediaModule.Application.Interfaces.Repositories;
using MediaModule.Domain.Entities;
using MediaModule.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Dtos;

namespace MediaModule.Persistence.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly MediaDbContext _dbContext;
        private readonly ILogger<MediaRepository> _logger;
        public MediaRepository(MediaDbContext dbContext,
            ILogger<MediaRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddMediaAsync(Media media, CancellationToken cancellationToken)
        {
            var exist = await _dbContext.Medias.AnyAsync(m => m.Url == media.Url, cancellationToken);
            if (exist)
            {
                _logger.LogWarning("[Media Module(Repository)] Media with Url:{Url} already exist.", media.Url);
                throw new MediaAlreadyExistException($"Media with Url:{media.Url} already exist");
            }
            await _dbContext.Medias.AddAsync(media, cancellationToken);
        }

        public async Task DeleteMediaAsync(Guid mediaId, CancellationToken cancellationToken)
        {
            var deleteResult = await _dbContext.Medias
                .Where(m => m.Id == mediaId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task DeleteEntityMediasAsync(Guid userId, CancellationToken cancellationToken)
        {
            var deleteResult = await _dbContext.Medias
                .Where(m => m.EntityId == userId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<Dictionary<Guid, MediaDto>> GetMainMediaByEntityIdsAsync(IEnumerable<Guid> entityIds, CancellationToken cancellationToken)
        {
            return await _dbContext.Medias
                .Where(m => !m.IsDeleted && m.IsMain && entityIds.Contains(m.EntityId))
                .ToDictionaryAsync(
                    m => m.EntityId,
                    m => new MediaDto
                    {
                        Id = m.Id,
                        Url = m.Url,
                        IsMain = m.IsMain
                    },
                    cancellationToken);
        }

        public async Task<Media> GetMainMediaByEntityIdAsync(Guid entityId, CancellationToken cancellationToken)
        {
            var media = await _dbContext.Medias
                .FirstOrDefaultAsync(m => m.EntityId == entityId && m.IsMain && !m.IsDeleted, cancellationToken);

            if (media == null)
            {
                _logger.LogWarning("[Media Module(Repository)] Main media for EntityId:{entityId} not found.", entityId);
                throw new MediaNotFoundException($"Main media for EntityId:{entityId} not found");
            }
            return media;
        }

        public async Task<Media> GetMediaByIdAsync(Guid mediaId, CancellationToken cancellationToken)
        {
            var media = await _dbContext.Medias.FirstOrDefaultAsync(m => m.Id == mediaId, cancellationToken);
            if (media == null)
            {
                _logger.LogWarning("[Media Module(Repository)] Media with Id:{mediaId} not found.", mediaId);
                throw new MediaNotFoundException($"Media with Id:{mediaId} not found");
            }
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
