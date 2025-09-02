using UserModule.Application.Dtos.Responses;

namespace UserModule.Application.Interfaces
{
    public interface IUserBlockService
    {
        Task BlockUser(Guid userId, Guid blockedUserId, CancellationToken cancellationToken);
        Task UnblockUser(Guid userId, Guid blockedUserId, CancellationToken cancellationToken);
        Task<IEnumerable<BlockedUserResponse>> GetBlockedUsers(Guid userId, CancellationToken cancellationToken);
    }
}
