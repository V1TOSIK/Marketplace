using AuthModule.Domain.Entities;

namespace AuthModule.Domain.Interfaces
{
    public interface IAuthUserRepository
    {
        Task<AuthUser> GetByIdAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false);
        Task<AuthUser?> GetByEmailAsync(string email, CancellationToken cancellationToken, bool includeDeleted = false);
        Task<AuthUser?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken, bool includeDeleted = false);
        Task AddAsync(AuthUser user, CancellationToken cancellationToken);
        Task HardDeleteAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber, CancellationToken cancellationToken);
        Task<bool> IsExistsAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> IsExistsAsync(string email, string phoneNumber, CancellationToken cancellationToken);

    }
}
