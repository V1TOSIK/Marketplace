using UserModule.Application.Dtos.Requests;
using UserModule.Application.Dtos.Responses;

namespace UserModule.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllUsers();
        Task<UserResponse> GetProfile(Guid userId);
        Task CreateNewProfile(Guid userId, CreateUserRequest request);
        Task UpdateProfile(Guid userId, UpdateUserRequest request);
        Task HardDeleteProfile(Guid userId);
        Task SoftDeleteProfile(Guid userId);
        Task AddPhoneNumber(Guid userId, string phone);
        Task RemovePhoneNumber(Guid userId, int phoneId);
    }
}
