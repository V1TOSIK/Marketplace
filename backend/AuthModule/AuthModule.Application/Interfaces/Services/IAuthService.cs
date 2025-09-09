using AuthModule.Application.Models;
using AuthModule.Domain.Entities;

namespace AuthModule.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthUser?> GetUserByCredential(string credential, CancellationToken cancellationToken = default);
        Task<RefreshToken> GenerateRefreshToken(Guid userId, string device, string ipAddress, Guid? tokenId = null, CancellationToken cancellationToken = default);
        AuthResult? CheckIfInvalid(AuthUser user);
        Task<AuthResult> BuildAuthResult(AuthUser user, Guid? tokenId = null, CancellationToken cancellationToken = default);
    }
}
