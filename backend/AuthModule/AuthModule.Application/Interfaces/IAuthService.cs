using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;

namespace AuthModule.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthorizeResponse> Login(LoginRequest request);
        Task<AuthorizeResponse> Register(RegisterRequest request);
        Task LogoutFromAllDevices(Guid userId);
        Task LogoutFromDevice(string refreshToken);
        Task<AuthorizeResponse> RefreshTokens(string refreshToken);
    }
}
