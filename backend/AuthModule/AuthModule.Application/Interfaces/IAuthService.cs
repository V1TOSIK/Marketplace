using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Models;

namespace AuthModule.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> Login(LoginRequest request, ClientInfo client);
        Task<AuthResult> Register(RegisterRequest request, ClientInfo client);
        Task<AuthResult> Restore(RestoreRequest request, ClientInfo client);
        Task ChangePassword(ChangePasswordRequest request, Guid userId);
        Task LogoutFromDevice(string refreshToken);
        Task LogoutFromAllDevices(Guid userId);
        Task<AuthResult> RefreshTokens(string refreshToken, ClientInfo client);
    }
}
