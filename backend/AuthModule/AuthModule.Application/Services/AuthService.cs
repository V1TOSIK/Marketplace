using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Exceptions;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Models;
using AuthModule.Domain.Entities;
using AuthModule.Domain.Exceptions;
using AuthModule.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;

namespace AuthModule.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRestorer _userRestorer;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        public AuthService(
            IAuthUserRepository authUserRepository,
            IPasswordHasher passwordHasher,
            IRefreshTokenRepository refreshTokenRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserRestorer userRestorer,
            IAuthUnitOfWork unitOfWork,
            ILogger<AuthService> logger)
        {
            _authUserRepository = authUserRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _userRestorer = userRestorer;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AuthResult> Login(LoginRequest request, ClientInfo client)
        {
            var user = await GetUserByCredentials(request.Email, request.PhoneNumber);

            user.EnsureCanLogin();
            if (!_passwordHasher.VerifyHashedPassword(user.Password.Value, request.Password))
            {
                _logger.LogWarning($"Invalid password attempt for user with ID {user.Id}.");
                throw new IncorrectCredentialsException("Invalid password.");
            }

            var refreshToken = await TakeToken(user.Id, client.Device, client.IpAddress);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"User with ID {user.Id} logged in successfully.");
            return new AuthResult
            {
                Response = new AuthorizeResponse
                {
                    UserId = user.Id,
                    Role = user.Role.ToString(),
                },
                RefreshToken = refreshToken,
            };
        }

        public async Task<AuthResult> Register(RegisterRequest request, ClientInfo client)
        {
            var hashPassword = _passwordHasher.HashPassword(request.Password);

            var email = !string.IsNullOrWhiteSpace(request.Email) ? request.Email : null;
            var phone = !string.IsNullOrWhiteSpace(request.PhoneNumber) ? request.PhoneNumber : null;


            if (email is null && phone is null)
            {
                _logger.LogError("Both email and phone number are null in register request.");
                throw new MissingAuthCredentialException();
            }

            AuthUser? existingUser = null;
            RefreshToken? refreshToken = null;

            if (email is not null)
                existingUser = await _authUserRepository.GetByEmailAsync(email, true);
            else if (phone is not null)
                existingUser = await _authUserRepository.GetByPhoneNumberAsync(phone, true);

            if (existingUser != null)
            {
                existingUser.EnsureCanLogin();

                throw email != null
                    ? new EmailAlreadyExistsException($"Email {email} is already registered.")
                    : new PhoneNumberAlreadyExistsException($"Phone number {phone} is already registered.");
            }

            var user = AuthUser.Create(
                email,
                phone,
                hashPassword,
                request.Role
            );

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _authUserRepository.AddAsync(user);

                refreshToken = RefreshToken.Create(user.Id, client.Device, client.IpAddress);

                await _refreshTokenRepository.AddAsync(refreshToken);
                _logger.LogInformation("Saving changes...");
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Changes saved.");
            });

            return new AuthResult()
            {
                Response = new AuthorizeResponse
                {
                    UserId = user.Id,
                    Role = user.Role.ToString()
                },
                RefreshToken = refreshToken,
            };
        }

        public async Task<AuthResult> Restore(RestoreRequest request, ClientInfo client)
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
                user = await _authUserRepository.GetByEmailAsync(email, true);
            else if (phone is not null)
                user = await _authUserRepository.GetByPhoneNumberAsync(phone, true);

            if (user == null)
                throw new UserNotFoundException("User does not exist.");

            user.EnsureCanLogin();

            if (!_passwordHasher.VerifyHashedPassword(user.Password.Value, request.Password))
                throw new IncorrectCredentialsException("Invalid password.");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                user.Restore();
                await _userRestorer.RestoreUserAsync(user.Id);
                await _unitOfWork.SaveChangesAsync();

                refreshToken = await TakeToken(user.Id, client.Device, client.IpAddress);
                await _unitOfWork.SaveChangesAsync();
            });

            return new AuthResult
            {
                Response = new AuthorizeResponse
                {
                    UserId = user.Id,
                    Role = user.Role.ToString()
                },
                RefreshToken = refreshToken
            };
        }

        public async Task ChangePassword(ChangePasswordRequest request, Guid userId)
        {
            var user = await _authUserRepository.GetByIdAsync(userId, false);

            if (!_passwordHasher.VerifyHashedPassword(user.Password.Value, request.OldPassword))
            {
                _logger.LogWarning($"Invalid old password attempt for user with ID {user.Id}.");
                throw new IncorrectCredentialsException("Old password is incorrect.");
            }

            var newHashedPassword = _passwordHasher.HashPassword(request.NewPassword);

            user.UpdatePassword(newHashedPassword);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task LogoutFromDevice(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new InvalidRefreshTokenException("RefreshToken is not valid");

            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            token.Revoke();
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"User logged out from device with refresh token {refreshToken}.");
        }

        public async Task LogoutFromAllDevices(Guid userId)
        {
            var userExists = await _authUserRepository.IsExistsAsync(userId);
            if (!userExists)
            {
                _logger.LogWarning($"User with ID {userId} does not exist.");
                throw new UserNotFoundException($"User with ID {userId} does not exist.");
            }

            await _refreshTokenRepository.RevokeAllAsync(userId);
            _logger.LogInformation($"All devices logged out for user with ID {userId}.");
        }

        public async Task<AuthResult> RefreshTokens(string refreshToken, ClientInfo client)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null || token.IsRevoked)
            {
                _logger.LogWarning($"Refresh token {refreshToken} not found or already revoked.");
                throw new RefreshTokenOperationException("Refresh token not found or already revoked.");
            }

            if (token.ExpirationDate < DateTime.UtcNow)
            {
                token.Revoke();
                await _unitOfWork.SaveChangesAsync();
                _logger.LogWarning($"Refresh token {refreshToken} has expired.");
                throw new RefreshTokenOperationException("Refresh token has expired.");
            }

            var user = await _authUserRepository.GetByIdAsync(token.UserId);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {token.UserId} not found.");
                throw new UserOperationException("User not found.");
            }
            user.EnsureCanLogin();
            var newRefreshToken = await TakeToken(user.Id, client.Device, client.IpAddress, token.Id);
            token.Revoke();
            await _unitOfWork.SaveChangesAsync();

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

        private async Task<RefreshToken> TakeToken(Guid userId, string device, string ipAddress, Guid? tokenId = null)
        {
            var refreshToken = RefreshToken.Create(userId, device, ipAddress, tokenId);
            await _refreshTokenRepository.AddAsync(refreshToken);
            return refreshToken;
        }

        private async Task<AuthUser> GetUserByCredentials(string? email, string? phone)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var user = await _authUserRepository.GetByEmailAsync(email, false);
                if (user == null)
                    throw new UserNotFoundException($"User with email {email} is not registered.");

                return user;
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var user = await _authUserRepository.GetByPhoneNumberAsync(phone, false);
                if (user == null)
                    throw new UserNotFoundException($"User with phone {phone} is not registered.");

                return user;
            }

            _logger.LogError("Both email and phone number are null in login request.");
            throw new MissingAuthCredentialException();
        }
    }
}
