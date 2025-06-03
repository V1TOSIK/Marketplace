using AuthUserModule.Domain.Entities;

namespace AuthUserModule.Domain.Interfaces
{
    internal interface IAuthRepository
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
        Task DeleteUserAsync(Guid userId);
        Task BlockUserAsync(Guid userId);
    }
}
