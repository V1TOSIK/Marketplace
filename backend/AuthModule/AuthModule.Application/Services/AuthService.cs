using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Exceptions;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Models;
using AuthModule.Domain.Entities;
using AuthModule.Domain.Exceptions;
using AuthModule.Domain.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        public AuthService(
            IAuthUserRepository authUserRepository,
            IPasswordHasher passwordHasher,
            IRefreshTokenRepository refreshTokenRepository,
            IUserRestorer userRestorer,
            IUnitOfWork unitOfWork,
            ILogger<AuthService> logger)
        {
            _authUserRepository = authUserRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _userRestorer = userRestorer;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AuthResult> Register(RegisterRequest request)
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

            if (email is not null)
            {
                existingUser = await _authUserRepository.GetUserByEmailAsync(email, true);
            }
            else if (phone is not null)
            {
                existingUser = await _authUserRepository.GetUserByPhoneNumberAsync(phone, true);
            }

            if (existingUser != null)
            {
                if (existingUser.IsBaned)
                {
                    _logger.LogWarning($"User with {email ?? phone} is blocked.");
                    throw new UserOperationException("User is blocked.");
                }

                if (!existingUser.IsDeleted)
                {
                    if (email != null)
                    {
                        _logger.LogWarning($"Email {email} is already registered by another user.");
                        throw new EmailAlreadyExistsException($"Email {email} is already registered.");
                    }
                    else
                    {
                        _logger.LogWarning($"Phone number {phone} is already registered by another user.");
                        throw new PhoneNumberAlreadyExistsException($"Phone number {phone} is already registered.");
                    }
                }
                else
                {
                    await _unitOfWork.ExecuteInTransactionAsync(async () =>
                    {
                        existingUser.Restore();
                        existingUser.UpdatePassword(hashPassword);
                        existingUser.UpdateRole(request.Role);
                        if (email != null) existingUser.UpdateEmail(email);
                        if (phone != null) existingUser.UpdatePhoneNumber(phone);

                        await _authUserRepository.UpdateUserAsync(existingUser);
                        await _userRestorer.RestoreUserAsync(existingUser.UserId);
                    });

                    var refreshToken = RefreshToken.Create(existingUser.UserId);
                    await _refreshTokenRepository.AddAsync(refreshToken);

                    _logger.LogInformation($"User with {email ?? phone} restored and logged in successfully.");
                    return new AuthResult()
                    {
                        Response = new AuthorizeResponse
                        {
                            UserId = existingUser.UserId,
                            Role = existingUser.Role.ToString()
                        },
                        RefreshToken = refreshToken,
                    };
                }
            }

            var user = AuthUser.Create(
                email,
                phone,
                hashPassword,
                request.Role
            );

            await _authUserRepository.AddUserAsync(user);

            var refreshTokenNew = RefreshToken.Create(user.UserId);
            await _refreshTokenRepository.AddAsync(refreshTokenNew);

            _logger.LogInformation($"New user registered with {email ?? phone}.");
            return new AuthResult()
            {
                Response = new AuthorizeResponse
                {
                    UserId = user.UserId,
                    Role = user.Role.ToString()
                },
                RefreshToken = refreshTokenNew,
            };
        }

        public async Task<AuthResult> Login(LoginRequest request)
        {
            var user = await GetUserByCredentials(request.Email, request.PhoneNumber);

            if (user.IsDeleted)
            {
                _logger.LogWarning($"User with ID {user.UserId} is deleted.");
                throw new UserOperationException("User is deleted.");
            }

            if (user.IsBaned)
            {
                _logger.LogWarning($"User with ID {user.UserId} is blocked.");
                throw new UserOperationException("User is blocked.");
            }

            if (!_passwordHasher.VerifyHashedPassword(user.Password.Value, request.Password))
            {
                _logger.LogWarning($"Invalid password attempt for user with ID {user.UserId}.");
                throw new IncorrectCredentialsException("Invalid password.");
            }

            var refreshToken = RefreshToken.Create(user.UserId);
            await _refreshTokenRepository.AddAsync(refreshToken);

            _logger.LogInformation($"User with ID {user.UserId} logged in successfully.");
            return new AuthResult
            {
                Response = new AuthorizeResponse
                {
                    UserId = user.UserId,
                    Role = user.Role.ToString(),
                },
                RefreshToken = refreshToken,
            };
        }

        private async Task<AuthUser> GetUserByCredentials(string? email, string? phone)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var user = await _authUserRepository.GetUserByEmailAsync(email, false);
                if (user == null)
                {
                    throw new UserNotFoundException($"User with email {email} is not registered.");
                }

                return user;
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var user = await _authUserRepository.GetUserByPhoneNumberAsync(phone, false);
                if (user == null)
                    throw new UserNotFoundException($"User with phone {phone} is not registered.");
                return user;
            }
            _logger.LogError("Both email and phone number are null in login request.");
            throw new MissingAuthCredentialException();
        }

        public async Task LogoutFromAllDevices(Guid userId)
        {
            var userExists = await _authUserRepository.IsUserExistsAsync(userId);
            if (!userExists)
            {
                _logger.LogWarning($"User with ID {userId} does not exist.");
                throw new UserNotFoundException($"User with ID {userId} does not exist.");
            }

            await _refreshTokenRepository.RevokeAllAsync(userId);
            _logger.LogInformation($"All devices logged out for user with ID {userId}.");
        }

        public async Task LogoutFromDevice(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new InvalidRefreshTokenException("RefreshToken is not valid");

            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null)
            {
                _logger.LogWarning($"Refresh token {refreshToken} not found or already revoked.");
                throw new RefreshTokenOperationException("Refresh token not found or already revoked.");
            }

            await _refreshTokenRepository.RevokeAsync(token.Id);
            _logger.LogInformation($"User logged out from device with refresh token {refreshToken}.");
        }

        public async Task<AuthResult> RefreshTokens(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogError("Refresh token is null or empty.");
                throw new InvalidRefreshTokenException("RefreshToken is not valid");
            }

            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null || token.IsRevoked)
            {
                _logger.LogWarning($"Refresh token {refreshToken} not found or already revoked.");
                throw new RefreshTokenOperationException("Refresh token not found or already revoked.");
            }

            if (token.ExpirationDate < DateTime.UtcNow)
            {
                await _refreshTokenRepository.RevokeAsync(token.Id);
                _logger.LogWarning($"Refresh token {refreshToken} has expired.");
                throw new RefreshTokenOperationException("Refresh token has expired.");
            }

            var user = await _authUserRepository.GetUserByIdAsync(token.UserId);

            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning($"User with ID {token.UserId} not found or deleted.");
                throw new UserOperationException("User not found or deleted.");
            }

            var newRefreshToken = RefreshToken.Create(user.UserId, token.Id);

            await _refreshTokenRepository.AddAsync(newRefreshToken);

            await _refreshTokenRepository.RevokeAsync(token.Id);

            _logger.LogInformation($"Refresh token for user with ID {user.UserId} refreshed successfully.");
            return new AuthResult
            {
                Response = new AuthorizeResponse
                {
                    UserId = user.UserId,
                    Role = user.Role.ToString()
                },

                RefreshToken = newRefreshToken
            };
        }

       
    }
}
