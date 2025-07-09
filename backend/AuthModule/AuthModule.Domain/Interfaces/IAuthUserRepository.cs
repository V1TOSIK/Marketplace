using AuthModule.Domain.Entities;

namespace AuthModule.Domain.Interfaces
{
    public interface IAuthUserRepository
    {
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
        Task<bool> IsUserExistsAsync(Guid userId);
        Task<bool> IsUserExistsAsync(string email, string phoneNumber);
        Task AddUserAsync(AuthUser user);
        Task<AuthUser?> GetUserByEmailAsync(string email, bool canBeDeleted);
        Task<AuthUser?> GetUserByPhoneNumberAsync(string phoneNumber, bool canBeDeleted);
        Task<AuthUser?> GetUserByIdAsync(Guid userId);
        Task UpdateUserAsync(AuthUser user);
        Task HardDeleteUserAsync(Guid userId);
        Task SoftDeleteUserAsync(Guid userId);
        Task RestoreUserAsync(Guid userId);

    }
}
