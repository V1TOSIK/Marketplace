using UserModule.Application.Dtos.Requests;
using UserModule.Application.Dtos.Responses;

namespace UserModule.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllUsers(CancellationToken cancellationToken);
        Task<UserResponse> GetProfile(Guid userId, CancellationToken cancellationToken);
        Task CreateNewProfile(Guid userId, CreateUserRequest request, CancellationToken cancellationToken);
        Task UpdateProfile(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken);
        Task HardDeleteProfile(Guid userId, CancellationToken cancellationToken);
        Task SoftDeleteProfile(Guid userId, CancellationToken cancellationToken);
        Task AddPhoneNumber(Guid userId, string phone, CancellationToken cancellationToken);
        Task RemovePhoneNumber(Guid userId, int phoneId, CancellationToken cancellationToken);
    }
}
