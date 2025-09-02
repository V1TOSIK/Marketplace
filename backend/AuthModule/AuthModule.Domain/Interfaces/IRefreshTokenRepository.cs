using AuthModule.Domain.Entities;

namespace AuthModule.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
        Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken);
    }
}
