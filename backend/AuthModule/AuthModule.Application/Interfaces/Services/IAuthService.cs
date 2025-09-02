using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Models;

namespace AuthModule.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginOrRegisterOAuth(LoginRequest request, ClientInfo client, CancellationToken cancellationToken);
        Task<AuthResult> Login(LoginRequest request, ClientInfo client, CancellationToken cancellationToken);
        Task<AuthResult> RegisterLocalUser(RegisterLocalRequest request, ClientInfo client, CancellationToken cancellationToken);
        Task<AuthResult> Restore(RestoreRequest request, ClientInfo client, CancellationToken cancellationToken);
        Task ChangePassword(ChangePasswordRequest request, Guid userId, CancellationToken cancellationToken);
        Task LogoutFromDevice(string refreshToken, CancellationToken cancellationToken);
        Task LogoutFromAllDevices(Guid userId, CancellationToken cancellationToken);
        Task<AuthResult> RefreshTokens(string refreshToken, ClientInfo client, CancellationToken cancellationToken);
        Task AddEmailAsync(Guid userId, string email, CancellationToken cancellationToken);
        Task AddPhoneAsync(Guid userId, string phone, CancellationToken cancellationToken);
    }
}
