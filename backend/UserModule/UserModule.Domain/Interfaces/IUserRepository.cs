using UserModule.Domain.Entities;

namespace UserModule.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task SoftDeleteAsync(Guid userId);
        Task HardDeleteAsync(Guid userId);
        Task<bool> ExistsAsync(Guid userId);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
    }
}
