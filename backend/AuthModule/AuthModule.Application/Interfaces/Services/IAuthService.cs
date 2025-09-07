using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Models;
using AuthModule.Domain.Entities;

namespace AuthModule.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginOrRegisterOAuth(OAuthLoginRequest request, ClientInfo client, CancellationToken cancellationToken = default);
        Task<AuthResult> Restore(RestoreRequest request, ClientInfo client, CancellationToken cancellationToken = default);
        Task ChangePassword(ChangePasswordRequest request, Guid userId, CancellationToken cancellationToken = default);
        Task LogoutFromDevice(string refreshToken, CancellationToken cancellationToken = default);
        Task LogoutFromAllDevices(Guid userId, CancellationToken cancellationToken = default);
        Task<AuthResult> RefreshTokens(string refreshToken, ClientInfo client, CancellationToken cancellationToken = default);
        Task AddEmailAsync(Guid userId, string email, CancellationToken cancellationToken = default);
        Task AddPhoneAsync(Guid userId, string phone, CancellationToken cancellationToken = default);
        Task<AuthUser?> GetUserByCredential(string credential, CancellationToken cancellationToken = default);
        Task<RefreshToken> GenerateRefreshToken(Guid userId, string device, string ipAddress, Guid? tokenId = null, CancellationToken cancellationToken = default);
        AuthResult? ThrowIfInvalid(AuthUser user);
        Task<AuthResult> BuildAuthResult(AuthUser user, bool saveChanges = true, CancellationToken cancellationToken = default);
    }
}
