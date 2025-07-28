using MediaModule.Domain.Interfaces;
using MediaModule.Domain.Entities;
using MediaModule.Domain.Enums;
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

        public async Task AddMediaAsync(Media media)
        {
            var exist = await _dbContext.Medias.AnyAsync(m => m.Id == media.Id);
            if (exist)
                throw new MediaAlreadyExistException($"Media with Id:{media.Id} already exist");
            await _dbContext.Medias.AddAsync(media);
        }

        public async Task DeleteMediaAsync(Guid mediaId)
        {
            var deleteResult = await _dbContext.Medias
                .Where(m => m.Id == mediaId)
                .ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Media>> GetAllMediaAsync()
        {
            var medias = await _dbContext.Medias
                .Where(m => !m.IsDeleted)
                .ToListAsync();

            return medias;
        }

        public async Task<Dictionary<Guid, string>> GetMainMediaUrlByEntityIdsAsync(IEnumerable<Guid> entityIds)
        {
            return await _dbContext.Medias
                .Where(m => !m.IsDeleted && m.IsMain && entityIds.Contains(m.EntityId))
                .ToDictionaryAsync(
                    m => m.EntityId,
                    m => m.Url);
        }

        public async Task<Media?> GetMainMediaByEntityIdAsync(Guid entityId)
        {
            return await _dbContext.Medias
                .Where(m => m.EntityId == entityId && m.IsMain && !m.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Media> GetMediaByIdAsync(Guid mediaId)
        {
            var media = await _dbContext.Medias.FirstOrDefaultAsync(m => m.Id == mediaId);
            if (media == null)
                throw new MediaNotFoundException($"Media with Id:{mediaId} not found");
            return media;
        }

        public async Task<IEnumerable<Media>> GetMediaByTypeAsync(string type)
        {
            var medias = await _dbContext.Medias
                .Where(m => m.MediaType == type)
                .ToListAsync();
            return medias;
        }

        public async Task<IEnumerable<Media>> GetMediasByEntityIdAsync(Guid entityId)
        {
            var medias = await _dbContext.Medias
                .Where(m => m.EntityId == entityId && !m.IsDeleted)
                .ToListAsync();
            return medias;
        }
    }
}
