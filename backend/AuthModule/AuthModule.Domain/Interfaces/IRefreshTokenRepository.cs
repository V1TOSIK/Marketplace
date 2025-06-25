using AuthModule.Domain.Entities;

namespace AuthModule.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);
        Task AddAsync(RefreshToken token);
        Task RevokeAsync(Guid tokenId);
        Task RevokeAllAsync(Guid userId);
        Task DeleteExpiredAsync();
    }
}
