using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Exceptions;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Models;
using AuthModule.Domain.Entities;
using AuthModule.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;

namespace AuthModule.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly IJwtProvider _jwtProvider;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AuthService> _logger;
        public AuthService(
            IAuthUserRepository authUserRepository,
            IPasswordHasher passwordHasher,
            IRefreshTokenRepository refreshTokenRepository,
            IAuthUnitOfWork unitOfWork,
            IJwtProvider jwtProvider,
            ICurrentUserService currentUserService,
            ILogger<AuthService> logger)
        {
            _authUserRepository = authUserRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _jwtProvider = jwtProvider;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<AuthResult> LoginOrRegisterOAuth(OAuthLoginRequest request, ClientInfo client, CancellationToken cancellationToken = default)
        {
            return null;
        }

        public async Task<AuthResult> RegisterOAuthUser(RegisterOAuthRequest request, ClientInfo client, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrWhiteSpace(request.ProviderUserId) && string.IsNullOrWhiteSpace(request.Provider))
            {
                _logger.LogError("Provider or ProviderUserId is null in OAuth register request.");
                throw new MissingAuthCredentialException("Provider or ProviderUserId is null in OAuth register request.");
            }

            var user = await _authUserRepository.GetByProviderAsync(request.ProviderUserId, request.Provider, true, true, cancellationToken);

            if (user != null)
            {
                user.ThrowIfCannotLogin();

                throw new OAuthUserAlreadyExistsException($"User with provider user id {request.ProviderUserId} already exists.");
            }

            user = AuthUser.CreateOAuth(
                request.ProviderUserId,
                request.Email,
                "User",
                request.Provider
            );
            RefreshToken? refreshToken = null;
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _authUserRepository.AddAsync(user, cancellationToken);
                refreshToken = RefreshToken.Create(user.Id, client.Device, client.IpAddress);
                await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
                _logger.LogInformation("Saving changes...");
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Changes saved.");
            }, cancellationToken);

            return new AuthResult()
            {
                Response = new AuthorizeResponse
                {
                    UserId = user.Id,
                    Role = user.Role.ToString()
                },
                RefreshToken = refreshToken ?? throw new RefreshTokenOperationException("Refresh token generation failed during register.")
            };
        }

        public async Task<AuthResult> Restore(RestoreRequest request, ClientInfo client, CancellationToken cancellationToken = default)
        {
            var email = !string.IsNullOrWhiteSpace(request.Email) ? request.Email : null;
            var phone = !string.IsNullOrWhiteSpace(request.PhoneNumber) ? request.PhoneNumber : null;

            if (email is null && phone is null)
            {
                _logger.LogError("Both email and phone number are null in restore request.");
                throw new MissingAuthCredentialException();
            }

            AuthUser? user = null;
            RefreshToken? refreshToken = null;

            if (email is not null)
                user = await _authUserRepository.GetByEmailAsync(email, true, true, cancellationToken);
            else if (phone is not null)
                user = await _authUserRepository.GetByPhoneNumberAsync(phone, true, true, cancellationToken);

            if (user == null)
                throw new UserNotFoundException("User does not exist.");

            if (!_passwordHasher.VerifyHashedPassword(user.Password!.Value, request.Password))
                throw new IncorrectCredentialsException("Invalid password.");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                user.Restore();
                refreshToken = await GenerateRefreshToken(user.Id, client.Device, client.IpAddress, cancellationToken: cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

            return new AuthResult
            {
                Response = new AuthorizeResponse
                {
                    UserId = user.Id,
                    Role = user.Role.ToString()
                },
                RefreshToken = refreshToken ?? throw new RefreshTokenOperationException("Refresh token generation failed during restore.")
            };
        }

        public async Task ChangePassword(ChangePasswordRequest request, Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _authUserRepository.GetByIdAsync(userId, false, false, cancellationToken);

            user.ThrowIfCannotLogin();
            if (user.IsOAuth())
                throw new OAuthUserCannotChangePasswordException("User was registered by OAuth and have not password");

            if (!_passwordHasher.VerifyHashedPassword(user.Password?.Value ?? string.Empty, request.OldPassword))
            {
                _logger.LogWarning($"Invalid password attempt for user with ID {user.Id}.");
                throw new IncorrectCredentialsException("Invalid password.");
            }

            var newHashedPassword = _passwordHasher.HashPassword(request.NewPassword);

            user.UpdatePassword(newHashedPassword);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task LogoutFromDevice(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new InvalidRefreshTokenException("RefreshToken is not valid");

            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
            token.Revoke();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"User logged out from device with refresh token {refreshToken}.");
        }

        public async Task LogoutFromAllDevices(Guid userId, CancellationToken cancellationToken = default)
        {
            var userExists = await _authUserRepository.IsExistsAsync(userId, cancellationToken);
            if (!userExists)
            {
                _logger.LogWarning($"User with ID {userId} does not exist.");
                throw new UserNotFoundException($"User with ID {userId} does not exist.");
            }

            await _refreshTokenRepository.RevokeAllAsync(userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"All devices logged out for user with ID {userId}.");
        }

        public async Task<AuthResult> RefreshTokens(string refreshToken, ClientInfo client, CancellationToken cancellationToken = default)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

            if (token == null || token.IsRevoked)
            {
                _logger.LogWarning($"Refresh token {refreshToken} not found or already revoked.");
                throw new RefreshTokenOperationException("Refresh token not found or already revoked.");
            }

            if (token.ExpirationDate < DateTime.UtcNow)
            {
                token.Revoke();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogWarning($"Refresh token {refreshToken} has expired.");
                throw new RefreshTokenOperationException("Refresh token has expired.");
            }

            var user = await _authUserRepository.GetByIdAsync(token.UserId, false, false, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {token.UserId} not found.");
                throw new UserOperationException("User not found.");
            }
            user.ThrowIfCannotLogin();
            var newRefreshToken = await GenerateRefreshToken(user.Id, client.Device, client.IpAddress, token.Id, cancellationToken);
            token.Revoke();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Refresh token for user with ID {user.Id} refreshed successfully.");
            return new AuthResult
            {
                Response = new AuthorizeResponse
                {
                    UserId = user.Id,
                    Role = user.Role.ToString()
                },

                RefreshToken = newRefreshToken
            };
        }

        public async Task AddEmailAsync(Guid userId, string email, CancellationToken cancellationToken = default)
        {
            var user = await _authUserRepository.GetByIdAsync(userId, false, false, cancellationToken);
            if (await _authUserRepository.IsEmailRegisteredAsync(email, cancellationToken))
            {
                _logger.LogWarning($"Email {email} is already registered.");
                throw new EmailAlreadyExistsException($"Email {email} is already registered.");
            }
            user.AddEmail(email);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task AddPhoneAsync(Guid userId, string phone, CancellationToken cancellationToken = default)
        {
            var user = await _authUserRepository.GetByIdAsync(userId, false, false, cancellationToken);
            if (await _authUserRepository.IsPhoneNumberRegisteredAsync(phone, cancellationToken))
            {
                _logger.LogWarning($"Phone number {phone} is already registered.");
                throw new PhoneNumberAlreadyExistsException($"Phone number {phone} is already registered.");
            }
            user.AddPhone(phone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<AuthUser?> GetUserByCredential(string credential, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(credential))
            {
                if(credential.Contains("@"))
                {
                    return await _authUserRepository.GetByEmailAsync(credential, true, true, cancellationToken);
                }
                else
                {
                    return await _authUserRepository.GetByPhoneNumberAsync(credential, true, true, cancellationToken);
                }
            }

            _logger.LogError("Credential is null or empty.");
            throw new MissingAuthCredentialException("Credential is null or empty");
        }

        public async Task<RefreshToken> GenerateRefreshToken(Guid userId, string device, string ipAddress, Guid? tokenId = null, CancellationToken cancellationToken = default)
        {
            var refreshToken = RefreshToken.Create(userId, device, ipAddress, tokenId);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            _logger.LogInformation($"New refresh token created for user with ID {userId}.");
            return refreshToken;
        }

        public AuthResult? ThrowIfInvalid(AuthUser user)
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
                        CanRestore = true,
                        Message = $"Ваш акаунт знаходиться на видаленні. До повного видалення акаунту залишилося днів: {timeLeft.Value.Days}"
                    },
                    RefreshToken = null
                };
            }
            if (user.IsBanned)
            {
                return new AuthResult
                {
                    Response = new AuthorizeResponse
                    {
                        UserId = user.Id,
                        IsBanned = user.IsBanned,
                        Message = $"Ваш акаунт заблоковано по причині: {user.BanReason}."
                    },
                    RefreshToken = null
                };
            }
            return null;
        }

        public async Task<AuthResult> BuildAuthResult(AuthUser user, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            var refreshToken = await GenerateRefreshToken(
                user.Id,
                _currentUserService.Device ?? "",
                _currentUserService.IpAddress ?? "",
                cancellationToken: cancellationToken);

            var accessToken = _jwtProvider.GenerateAccessToken(user.Id, user.Role.ToString());

            if (saveChanges)
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
    }
}
