using AuthModule.Domain.Entities;

namespace AuthModule.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
        Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken);
        Task RevokeByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken);
        Task DeleteExpiredAsync(CancellationToken cancellationToken);
    }
}
