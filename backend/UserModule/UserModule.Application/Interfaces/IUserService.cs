using UserModule.Application.Dtos.Requests;
using UserModule.Application.Dtos.Responses;

namespace UserModule.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> GetProfile(Guid userId, CancellationToken cancellationToken);
        Task CreateNewProfile(Guid userId, CreateUserRequest request, CancellationToken cancellationToken);
        Task UpdateProfile(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken);
        Task BanProfile(Guid userId, CancellationToken cancellationToken);
        Task UnBanProfile(Guid userId, CancellationToken cancellationToken);
        Task SoftDeleteProfile(Guid userId, CancellationToken cancellationToken);
        Task HardDeleteProfile(Guid userId, CancellationToken cancellationToken);
    }
}
