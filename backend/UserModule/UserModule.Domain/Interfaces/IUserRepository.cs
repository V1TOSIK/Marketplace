using UserModule.Domain.Entities;

namespace UserModule.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid userId, bool includeDeleted);
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        Task SoftDeleteAsync(Guid userId);
        Task HardDeleteAsync(Guid userId);
        Task<bool> ExistsAsync(Guid userId);
        Task<IEnumerable<string>> GetPhoneNumbersAsync(Guid userId);
    }
}
