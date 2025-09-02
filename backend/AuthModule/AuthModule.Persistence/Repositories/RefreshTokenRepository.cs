using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Domain.Entities;
using AuthModule.Domain.Exceptions;
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
        public async Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            var refreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.ExpirationDate > DateTime.UtcNow, cancellationToken);

            if (refreshToken == null)
            {
                _logger.LogError("Refresh token not found");
                throw new RefreshTokenNotFoundException("Refresh token not found");
            }

            return refreshToken;
        }

        public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
        {
            await _dbContext.RefreshTokens.AddAsync(token, cancellationToken);
            _logger.LogInformation($"Refresh token added for user {token.UserId} with expiration date {token.ExpirationDate}", cancellationToken);
        }

        public async Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken)
        {
            await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(rt => rt.IsRevoked, true)
                .SetProperty(rt => rt.RevokedAt, DateTime.UtcNow),
                cancellationToken
            );

            _logger.LogInformation($"All refresh tokens for user {userId} revoked successfully.", cancellationToken);
        }
    }
}
