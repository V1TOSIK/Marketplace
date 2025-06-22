using AuthUserModule.Domain.Entities;
using AuthUserModule.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthUserModule.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _dbContext;
        public RefreshTokenRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(RefreshToken token)
        {
            await _dbContext.RefreshTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteExpiredAsync()
        {
            var deleteResult = await _dbContext.RefreshTokens
                .Where(rt => rt.ExpirationDate < DateTime.UtcNow)
                .ExecuteDeleteAsync();

            if (deleteResult == 0)
                throw new InvalidOperationException("No expired refresh tokens found to delete.");
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
        }

        public async Task RevokeAsync(Guid tokenId)
        {
            var token = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Id == tokenId && !rt.IsRevoked);

            if (token == null)
                throw new InvalidOperationException($"Refresh token with ID {tokenId} does not exist or is already revoked.");
            
            token.Revoke();

            await _dbContext.SaveChangesAsync();
        }
    }
}
