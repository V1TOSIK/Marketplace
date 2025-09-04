using UserModule.Domain.Entities;

namespace UserModule.Application.Interfaces.Repositories
{
    public interface IUserBlockRepository
    {
        Task<UserBlock> GetActiveBlockAsync(Guid userId, Guid blockedUserId, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetBlockedUsersAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(UserBlock block, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid userId, Guid blockedUserId, CancellationToken cancellationToken);
    }
}
