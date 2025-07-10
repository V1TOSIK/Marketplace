using AuthModule.Domain.Entities;

namespace AuthModule.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);
        Task AddAsync(RefreshToken token);
        Task DeleteExpiredAsync();
        Task RevokeExpiredAsync();
        Task RevokeAllAsync(Guid userId);
        Task RevokeAsync(string token);
    }
}
