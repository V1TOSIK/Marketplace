using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Models;

namespace AuthModule.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> Login(LoginRequest request);
        Task<AuthResult> Register(RegisterRequest request);
        Task LogoutFromAllDevices(Guid userId);
        Task LogoutFromDevice(string refreshToken);
        Task<AuthResult> RefreshTokens(string refreshToken);
    }
}
