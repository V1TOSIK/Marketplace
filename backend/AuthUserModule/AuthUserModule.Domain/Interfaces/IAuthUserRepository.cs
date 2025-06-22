using AuthUserModule.Domain.Entities;

namespace AuthUserModule.Domain.Interfaces
{
    public interface IAuthUserRepository
    {
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
        Task<bool> IsUserExistsAsync(Guid userId);
        Task<bool> IsUserExistsAsync(string email, string phoneNumber);
        Task AddUserAsync(AuthUser user);
        Task<AuthUser?> GetUserByEmailAsync(string email);
        Task<AuthUser?> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<AuthUser?> GetUserByIdAsync(Guid userId);
        Task UpdateUserAsync(AuthUser user);
        Task HardDeleteUserAsync(Guid userId);
        Task SoftDeleteUserAsync(Guid userId);
        Task RestoreUserAsync(Guid userId);

    }
}
