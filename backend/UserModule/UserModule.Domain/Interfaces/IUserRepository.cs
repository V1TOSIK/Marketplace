using UserModule.Domain.Entities;

namespace UserModule.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task HardDeleteAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
