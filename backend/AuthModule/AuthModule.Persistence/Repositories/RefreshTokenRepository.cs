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
        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            var refreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.ExpirationDate > DateTime.UtcNow);

            if (refreshToken == null)
            {
                _logger.LogError("Refresh token not found");
                throw new RefreshTokenNotFoundException("Refresh token not found");
            }

            return refreshToken;
        }

        public async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _dbContext.RefreshTokens.AddAsync(token);
            _logger.LogInformation($"Refresh token added for user {token.UserId} with expiration date {token.ExpirationDate}");
        }

        public async Task DeleteExpiredAsync()
        {
            var deleteResult = await _dbContext.RefreshTokens
                .Where(rt => rt.ExpirationDate < DateTime.UtcNow)
                .ExecuteDeleteAsync();

            if (deleteResult == 0)
            {
                _logger.LogWarning("No expired refresh tokens found to delete.");
                throw new RefreshTokenNotFoundException("No expired refresh tokens found to delete.");
            }

            _logger.LogInformation($"Expired refresh tokens deleted.");
        }

        public async Task RevokeExpiredAsync()
        {
            var expiredTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.ExpirationDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in expiredTokens)
            {
                token.Revoke();
            }
            _logger.LogInformation($"{expiredTokens.Count} refresh tokens was expired and been revoked");
        }

        public async Task RevokeAllAsync(Guid userId)
        {
            await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(rt => rt.IsRevoked, true)
                .SetProperty(rt => rt.RevokedAt, DateTime.UtcNow)
            );

            _logger.LogInformation("All refresh tokens for user {UserId} revoked successfully.", userId);
        }

        public async Task RevokeAsync(string token)
        {
            var refreshToken = await GetByTokenAsync(token);
            
            refreshToken.Revoke();

            _logger.LogInformation($"Refresh token: {refreshToken.Token} revoked successfully.");
        }

    }
}
