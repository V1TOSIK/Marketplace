using UserModule.Application.Dtos.Responses;

namespace UserModule.Application.Interfaces
{
    public interface IUserBlockService
    {
        Task BlockUser(Guid userId, Guid blockedUserId);
        Task UnblockUser(Guid userId, Guid blockedUserId);
        Task<IEnumerable<BlockedUserResponse>> GetBlockedUsers(Guid userId);
    }
}
