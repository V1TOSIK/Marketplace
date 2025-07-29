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

        public async Task<AuthResult> Login(LoginRequest request, ClientInfo client, CancellationToken cancellationToken)
        {
            var user = await GetUserByCredentials(request.Email, request.PhoneNumber, cancellationToken);

            user.ThrowIfCannotLogin();
            if (!_passwordHasher.VerifyHashedPassword(user.Password.Value, request.Password))
            {
                _logger.LogWarning($"Invalid password attempt for user with ID {user.Id}.");
                throw new IncorrectCredentialsException("Invalid password.");
            }

            var refreshToken = await TakeToken(user.Id, client.Device, client.IpAddress, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

        public async Task<AuthResult> Register(RegisterRequest request, ClientInfo client, CancellationToken cancellationToken)
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
                existingUser = await _authUserRepository.GetByEmailAsync(email, cancellationToken, true);
            else if (phone is not null)
                existingUser = await _authUserRepository.GetByPhoneNumberAsync(phone, cancellationToken, true);

            if (existingUser != null)
            {
                existingUser.ThrowIfCannotLogin();

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
                RefreshToken = refreshToken,
            };
        }

        public async Task<AuthResult> Restore(RestoreRequest request, ClientInfo client, CancellationToken cancellationToken)
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
                user = await _authUserRepository.GetByEmailAsync(email, cancellationToken, true);
            else if (phone is not null)
                user = await _authUserRepository.GetByPhoneNumberAsync(phone, cancellationToken, true);

            if (user == null)
                throw new UserNotFoundException("User does not exist.");

            user.ThrowIfCannotLogin();

            if (!_passwordHasher.VerifyHashedPassword(user.Password.Value, request.Password))
                throw new IncorrectCredentialsException("Invalid password.");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                user.Restore();
                await _userRestorer.RestoreUserAsync(user.Id);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                refreshToken = await TakeToken(user.Id, client.Device, client.IpAddress, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

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

        public async Task ChangePassword(ChangePasswordRequest request, Guid userId, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(userId, cancellationToken, false);

            if (!_passwordHasher.VerifyHashedPassword(user.Password.Value, request.OldPassword))
            {
                _logger.LogWarning($"Invalid old password attempt for user with ID {user.Id}.");
                throw new IncorrectCredentialsException("Old password is incorrect.");
            }

            var newHashedPassword = _passwordHasher.HashPassword(request.NewPassword);

            user.UpdatePassword(newHashedPassword);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task LogoutFromDevice(string refreshToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new InvalidRefreshTokenException("RefreshToken is not valid");

            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
            token.Revoke();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"User logged out from device with refresh token {refreshToken}.");
        }

        public async Task LogoutFromAllDevices(Guid userId, CancellationToken cancellationToken)
        {
            var userExists = await _authUserRepository.IsExistsAsync(userId, cancellationToken);
            if (!userExists)
            {
                _logger.LogWarning($"User with ID {userId} does not exist.");
                throw new UserNotFoundException($"User with ID {userId} does not exist.");
            }

            await _refreshTokenRepository.RevokeAllAsync(userId, cancellationToken);
            _logger.LogInformation($"All devices logged out for user with ID {userId}.");
        }

        public async Task<AuthResult> RefreshTokens(string refreshToken, ClientInfo client, CancellationToken cancellationToken)
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

            var user = await _authUserRepository.GetByIdAsync(token.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {token.UserId} not found.");
                throw new UserOperationException("User not found.");
            }
            user.ThrowIfCannotLogin();
            var newRefreshToken = await TakeToken(user.Id, client.Device, client.IpAddress, cancellationToken, token.Id);
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

        private async Task<RefreshToken> TakeToken(Guid userId, string device, string ipAddress, CancellationToken cancellationToken, Guid? tokenId = null)
        {
            var refreshToken = RefreshToken.Create(userId, device, ipAddress, tokenId);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            return refreshToken;
        }

        private async Task<AuthUser> GetUserByCredentials(string? email, string? phone, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var user = await _authUserRepository.GetByEmailAsync(email, cancellationToken, false);
                if (user == null)
                    throw new UserNotFoundException($"User with email {email} is not registered.");

                return user;
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var user = await _authUserRepository.GetByPhoneNumberAsync(phone, cancellationToken, false);
                if (user == null)
                    throw new UserNotFoundException($"User with phone {phone} is not registered.");

                return user;
            }

            _logger.LogError("Both email and phone number are null in login request.");
            throw new MissingAuthCredentialException();
        }

        public async Task AddEmailAsync(Guid userId, string email, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(userId, cancellationToken, false);
            if (await _authUserRepository.IsEmailRegisteredAsync(email, cancellationToken))
            {
                _logger.LogWarning($"Email {email} is already registered.");
                throw new EmailAlreadyExistsException($"Email {email} is already registered.");
            }
            user.AddEmail(email);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task AddPhoneAsync(Guid userId, string phone, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(userId, cancellationToken, false);
            if (await _authUserRepository.IsPhoneNumberRegisteredAsync(phone, cancellationToken))
            {
                _logger.LogWarning($"Phone number {phone} is already registered.");
                throw new PhoneNumberAlreadyExistsException($"Phone number {phone} is already registered.");
            }
            user.AddPhone(phone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
