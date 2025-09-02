using UserModule.Domain.Entities;

namespace UserModule.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false, bool includeBanned = false);
        Task<User> GetByIdWithPhoneNumbersAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false, bool includeBanned = false);
        Task<IEnumerable<string>> GetUserPhoneNumbersAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task HardDeleteAsync(Guid userId, CancellationToken cancellationToken);
    }
}
