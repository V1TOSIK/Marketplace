using AuthUserModule.Application.Dtos.Requests;
using AuthUserModule.Application.Dtos.Responses;

namespace AuthUserModule.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthorizeUserResponse> Login(AuthorizeUserRequest request);
        Task<AuthorizeUserResponse> Register(RegisterUserRequest request);
        Task<bool> Logout(Guid userId);
    }
}
