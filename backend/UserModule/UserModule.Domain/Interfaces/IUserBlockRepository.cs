using UserModule.Domain.Entities;

namespace UserModule.Domain.Interfaces
{
    public interface IUserBlockRepository
    {
        Task<UserBlock> GetByIdAsync(int id);
        Task<UserBlock> GetActiveBlockAsync(Guid userId, Guid blockedUserId);
        Task<UserBlock> GetAnyBlockAsync(Guid userId, Guid blockedUserId);
        Task AddAsync(UserBlock block);
        Task<bool> ExistsAsync(Guid userId, Guid blockedUserId);
        Task<IEnumerable<User>> GetBlockedUsersAsync(Guid userId);
    }
}
