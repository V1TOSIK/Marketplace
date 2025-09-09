using AuthModule.Domain.Entities;

namespace AuthModule.Application.Interfaces.Repositories
{
    public interface IAuthUserRepository
    {
        Task<AuthUser> GetByIdAsync(Guid userId, bool includeDeleted = false, bool includeBanned = false, CancellationToken cancellationToken = default);
        Task<AuthUser?> GetByOAuthAsync(string providerUserId, string provider, bool includeDeleted = false, bool includeBanned = false, CancellationToken cancellationToken = default);
        Task<AuthUser?> GetByEmailAsync(string email, bool includeDeleted = false, bool includeBanned = false, CancellationToken cancellationToken = default);
        Task<AuthUser?> GetByPhoneNumberAsync(string phoneNumber, bool includeDeleted = false, bool includeBanned = false, CancellationToken cancellationToken = default);
        Task AddAsync(AuthUser user, CancellationToken cancellationToken);
        Task HardDeleteAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber, CancellationToken cancellationToken);
        Task<bool> IsOAuthRegisteredAsync(string providerUserId, string provider, CancellationToken cancellationToken);
        Task<AuthUser?> GetByCredentialAsync(string credential, bool includeDeleted = false, bool includeBanned = false, CancellationToken cancellationToken = default);

    }
}
