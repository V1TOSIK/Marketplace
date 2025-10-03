using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Models;
using AuthModule.Domain.Entities;
using AuthModule.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using SharedKernel.CurrentUser;

namespace AuthModule.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly IJwtProvider _jwtProvider;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AuthService> _logger;
        public AuthService(
            IAuthUserRepository authUserRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IAuthUnitOfWork unitOfWork,
            IJwtProvider jwtProvider,
            ICurrentUserService currentUserService,
            ILogger<AuthService> logger)
        {
            _authUserRepository = authUserRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _jwtProvider = jwtProvider;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<AuthResult> BuildAuthResult(AuthUser user, Guid deviceId, Guid? tokenId = null, CancellationToken cancellationToken = default)
        {
            var refreshToken = await GenerateRefreshToken(
                user.Id,
                _currentUserService.Device ?? "",
                deviceId,
                _currentUserService.IpAddress ?? "",
                tokenId,
                cancellationToken: cancellationToken);

            var accessToken = _jwtProvider.GenerateAccessToken(user.Id, user.Role.ToString());
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResult
            {
                Response = new AuthorizeResponse
                {
                    UserId = user.Id,
                    Role = user.Role.ToString(),
                    AccessToken = accessToken,
                },
                RefreshToken = refreshToken
            };
        }

        public async Task<RefreshToken> GenerateRefreshToken(Guid userId, string device, Guid deviceId, string ipAddress, Guid? tokenId = null, CancellationToken cancellationToken = default)
        {
            deviceId = deviceId == Guid.Empty ? Guid.NewGuid() : deviceId;

            if (deviceId != Guid.Empty)
                await _refreshTokenRepository.RevokeByDeviceIdAsync(deviceId, cancellationToken);

            var refreshToken = RefreshToken.Create(userId, device, deviceId, ipAddress, tokenId);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            _logger.LogInformation($"[Auth Module(AuthService)] New refresh token created for user with ID {userId}.");
            return refreshToken;
        }

        public async Task<AuthUser?> GetUserByCredential(string credential, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(credential))
            {
                if(credential.Contains("@"))
                {
                    return await _authUserRepository.GetByEmailAsync(credential, cancellationToken);
                }
                else
                {
                    return await _authUserRepository.GetByPhoneNumberAsync(credential, cancellationToken);
                }
            }

            _logger.LogError("[Auth Module(AuthService)] Credential is null or empty.");
            throw new MissingAuthCredentialException("Credential is null or empty");
        }

        public AuthResult? CheckIfInvalid(AuthUser user)
        {
            if (user.IsDeleted)
            {
                var deletionPeriod = TimeSpan.FromDays(7);
                var timeSinceDeletion = DateTime.UtcNow - user.DeletedAt;
                var timeLeft = deletionPeriod - timeSinceDeletion;

                if (timeLeft < TimeSpan.Zero)
                    timeLeft = TimeSpan.Zero;

                return new AuthResult
                {
                    Response = new AuthorizeResponse
                    {
                        UserId = user.Id,
                        IsDeleted = user.IsDeleted,
                        IsBanned = user.IsBanned,
                        Message = $"Ваш акаунт знаходиться на видаленні. До повного видалення акаунту залишилося днів: { timeLeft.Value.Days }"
                    },
                    RefreshToken = null!
                };
            }
            if (user.IsBanned)
            {
                return new AuthResult
                {
                    Response = new AuthorizeResponse
                    {
                        UserId = user.Id,
                        IsDeleted = user.IsDeleted,
                        IsBanned = user.IsBanned,
                        Message = $"Ваш акаунт заблоковано по причині: {user.BanReason}."
                    },
                    RefreshToken = null!
                };
            }
            return null;
        }
    }
}
