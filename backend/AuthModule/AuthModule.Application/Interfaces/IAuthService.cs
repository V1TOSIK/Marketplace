using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;

namespace AuthModule.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthorizeUserResponse> Login(AuthorizeUserRequest request);
        Task<AuthorizeUserResponse> Register(RegisterUserRequest request);
        Task<bool> Logout(Guid userId);
    }
}
