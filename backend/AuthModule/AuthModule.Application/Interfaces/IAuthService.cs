using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Models;

namespace AuthModule.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> Login(LoginRequest request, ClientInfo client, CancellationToken cancellationToken);
        Task<AuthResult> Register(RegisterRequest request, ClientInfo client, CancellationToken cancellationToken);
        Task<AuthResult> Restore(RestoreRequest request, ClientInfo client, CancellationToken cancellationToken);
        Task ChangePassword(ChangePasswordRequest request, Guid userId, CancellationToken cancellationToken);
        Task LogoutFromDevice(string refreshToken, CancellationToken cancellationToken);
        Task LogoutFromAllDevices(Guid userId, CancellationToken cancellationToken);
        Task<AuthResult> RefreshTokens(string refreshToken, ClientInfo client, CancellationToken cancellationToken);
        Task AddEmailAsync(Guid userId, string email, CancellationToken cancellationToken);
        Task AddPhoneAsync(Guid userId, string phone, CancellationToken cancellationToken);
    }
}
