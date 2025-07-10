using AuthModule.Domain.Entities;

namespace AuthModule.Domain.Interfaces
{
    public interface IAuthUserRepository
    {
        Task<AuthUser> GetByIdAsync(Guid userId, bool includeDeleted = false);
        Task<AuthUser?> GetByEmailAsync(string email, bool includeDeleted = false);
        Task<AuthUser?> GetByPhoneNumberAsync(string phoneNumber, bool includeDeleted = false);
        Task AddAsync(AuthUser user);
        Task HardDeleteAsync(Guid userId);
        Task SoftDeleteAsync(Guid userId);
        Task RestoreAsync(Guid userId);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
        Task<bool> IsExistsAsync(Guid userId);
        Task<bool> IsExistsAsync(string email, string phoneNumber);

    }
}
