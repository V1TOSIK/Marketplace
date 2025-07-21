using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserModule.Domain.Entities;
using UserModule.Domain.Exceptions;
using UserModule.Domain.Interfaces;

namespace UserModule.Persistence.Repositories
{
    public class UserBlockRepository : IUserBlockRepository
    {
        private readonly UserDbContext _dbContext;
        private readonly ILogger<UserBlockRepository> _logger;
        public UserBlockRepository(UserDbContext dbContext,
            ILogger<UserBlockRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<UserBlock> GetByIdAsync(int blockId)
        {
            var result = await _dbContext.UserBlocks
                .FirstOrDefaultAsync(ub => ub.Id == blockId);
            if (result == null)
                throw new BlockNotFoundException($"Block with Id:{blockId} not found");

            return result;
        }
        public async Task<UserBlock> GetActiveBlockAsync(Guid userId, Guid blockedUserId)
        {
            var result = await _dbContext.UserBlocks
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BlockedUserId == blockedUserId && ub.UnblockedAt == null);
            if (result == null)
                throw new BlockNotFoundException($"Active block not found for user: {blockedUserId}");

            return result;
        }

        public async Task<UserBlock> GetAnyBlockAsync(Guid userId, Guid blockedUserId)
        {
            var result = await _dbContext.UserBlocks
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BlockedUserId == blockedUserId);
            if (result == null)
                throw new BlockNotFoundException($"Block not found for user: {blockedUserId}");

            return result;
        }

        public async Task<IEnumerable<User>> GetBlockedUsersAsync(Guid userId)
        {
            var result = await _dbContext.UserBlocks
                .Where(ub => ub.UserId == userId && ub.UnblockedAt == null)
                .Join(
                    _dbContext.Users,
                    ub => ub.BlockedUserId,
                    u => u.Id,
                    (ub, u) => u
                )
                .AsNoTracking()
                .ToListAsync();
            return result;
        }

        public async Task AddAsync(UserBlock block)
        {
            if (await ExistsAsync(block.UserId, block.BlockedUserId))
            {
                _logger.LogError($"User with ID {block.BlockedUserId} already blocked.");
                throw new BlockExistException($"User with ID {block.BlockedUserId} already blocked.");
            }
            await _dbContext.UserBlocks.AddAsync(block);
            _logger.LogInformation($"User: {block.UserId} block user: {block.BlockedUserId}");
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid blockedUserId)
        {
            var result = await _dbContext.UserBlocks
                .AnyAsync(ub => ub.UserId == userId
                && ub.BlockedUserId == blockedUserId
                && ub.UnblockedAt == null);

            return result;
        }
    }
}
