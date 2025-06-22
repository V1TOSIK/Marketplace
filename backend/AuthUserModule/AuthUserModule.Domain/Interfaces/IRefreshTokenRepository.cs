using AuthUserModule.Domain.Entities;

namespace AuthUserModule.Domain.Interfaces
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
