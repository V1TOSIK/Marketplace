using UserModule.Application.Dtos;
using UserModule.Application.Dtos.Requests;

namespace UserModule.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto> GetProfile(Guid userId, CancellationToken cancellationToken);
        Task CreateNewProfile(Guid userId, CreateUserRequest request, CancellationToken cancellationToken);
        Task UpdateProfile(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken);
    }
}
