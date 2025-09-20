using AuthModule.Domain.Entities;

namespace AuthModule.Application.Interfaces.Repositories
{
    public interface IAuthUserRepository
    {
        Task<AuthUser> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<AuthUser?> GetByOAuthAsync(string providerUserId, string provider, CancellationToken cancellationToken = default);
        Task<AuthUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<AuthUser?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
        Task AddAsync(AuthUser user, CancellationToken cancellationToken = default);
        Task HardDeleteAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber, CancellationToken cancellationToken = default);
        Task<AuthUser?> GetByCredentialAsync(string credential, CancellationToken cancellationToken = default);

    }
}
