using AuthModule.Domain.Entities;
using AuthModule.Domain.Exceptions;
using AuthModule.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthModule.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _dbContext;
        private readonly ILogger<RefreshTokenRepository> _logger;
        public RefreshTokenRepository(AuthDbContext dbContext,
            ILogger<RefreshTokenRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task AddAsync(RefreshToken token)
        {
            await _dbContext.RefreshTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Refresh token added for user {UserId} with expiration date {ExpirationDate}",
                token.UserId, token.ExpirationDate);
        }

        public async Task RevokeExpiredAsync()
        {
            var deleteResult = await _dbContext.RefreshTokens
                .Where(rt => rt.ExpirationDate < DateTime.UtcNow)
                .ExecuteDeleteAsync();

            if (deleteResult == 0)
            {
                _logger.LogWarning("No expired refresh tokens found to delete.");
                throw new RefreshTokenOperationException("No expired refresh tokens found to delete.");
            }

            _logger.LogInformation("{Count} expired refresh tokens deleted.", deleteResult);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);
        }

        public async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();
        }

        public async Task RevokeAllAsync(Guid userId)
        {
            var tokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoke();
            }
            
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("All refresh tokens for user {UserId} revoked successfully.", userId);
        }

        public async Task RevokeAsync(Guid tokenId)
        {
            var token = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Id == tokenId && !rt.IsRevoked);

            if (token == null)
            {
                _logger.LogWarning("Refresh token with ID {TokenId} does not exist or is already revoked.", tokenId);
                throw new RefreshTokenOperationException($"Refresh token with ID {tokenId} does not exist or is already revoked.");
            }
            
            token.Revoke();

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Refresh token with ID {TokenId} revoked successfully.", tokenId);
        }

    }
}
