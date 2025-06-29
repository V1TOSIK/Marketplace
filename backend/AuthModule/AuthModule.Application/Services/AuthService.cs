using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Exceptions;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Models;
using AuthModule.Domain.Entities;
using AuthModule.Domain.Exceptions;
using AuthModule.Domain.Interfaces;

namespace AuthModule.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public AuthService(
            IAuthUserRepository authUserRepository,
            IPasswordHasher passwordHasher,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _authUserRepository = authUserRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthResult> Register(RegisterRequest request)
        {
            var hashPassword = _passwordHasher.HashPassword(request.Password);

            var email = !string.IsNullOrWhiteSpace(request.Email) ? request.Email : null;
            var phone = !string.IsNullOrWhiteSpace(request.PhoneNumber) ? request.PhoneNumber : null;

            if (email is null && phone is null)
                throw new MissingAuthCredentialException();

            if (email is not null && await _authUserRepository.IsEmailRegisteredAsync(email))
                throw new EmailAlreadyExistsException($"Email {email} is already registered.");

            if (phone is not null && await _authUserRepository.IsPhoneNumberRegisteredAsync(phone))
                throw new PhoneNumberAlreadyExistsException($"Phone number {phone} is already registered.");

            var user = AuthUser.Create(
                email,
                phone,
                hashPassword,
                request.Role
            );

            await _authUserRepository.AddUserAsync(user);

            var refreshToken = RefreshToken.Create(user.UserId);
            await _refreshTokenRepository.AddAsync(refreshToken);

            return new AuthResult()
            {
                Response = new AuthorizeResponse
                {
                    UserId = user.UserId,
                    Role = user.Role.ToString()
                },
                RefreshToken = refreshToken,
            };

        }

        public async Task<AuthResult> Login(LoginRequest request)
        {
            var user = await GetUserByCredentials(request.Email, request.PhoneNumber);

            if (user.IsDeleted)
                throw new UserOperationException("User is deleted.");

            if (!_passwordHasher.VerifyHashedPassword(user.Password.Value, request.Password))
                throw new IncorrectCredentialsException("Invalid password.");

            var refreshToken = RefreshToken.Create(user.UserId);
            await _refreshTokenRepository.AddAsync(refreshToken);

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
                var user = await _authUserRepository.GetUserByEmailAsync(email);
                if (user == null)
                    throw new UserNotFoundException($"User with email {email} is not registered.");
                return user;
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var user = await _authUserRepository.GetUserByPhoneNumberAsync(phone);
                if (user == null)
                    throw new UserNotFoundException($"User with phone {phone} is not registered.");
                return user;
            }

            throw new MissingAuthCredentialException();
        }

        public async Task LogoutFromAllDevices(Guid userId)
        {
            var userExists = await _authUserRepository.IsUserExistsAsync(userId);
            if (!userExists)
                throw new UserNotFoundException($"User with ID {userId} does not exist.");

            await _refreshTokenRepository.RevokeAllAsync(userId);
        }

        public async Task LogoutFromDevice(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new InvalidRefreshTokenException("RefreshToken is not valid");

            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null)
                throw new RefreshTokenOperationException("Refresh token not found or already revoked.");

            await _refreshTokenRepository.RevokeAsync(token.Id);
        }

        public async Task<AuthResult> RefreshTokens(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new InvalidRefreshTokenException("RefreshToken is not valid");

            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null || token.IsRevoked)
                throw new RefreshTokenOperationException("Refresh token not found or already revoked.");

            if (token.ExpirationDate < DateTime.UtcNow)
            {
                await _refreshTokenRepository.RevokeAsync(token.Id);
                throw new RefreshTokenOperationException("Refresh token has expired.");
            }

            var user = await _authUserRepository.GetUserByIdAsync(token.UserId);

            if (user == null || user.IsDeleted)
                throw new UserOperationException("User not found or deleted.");

            var newRefreshToken = RefreshToken.Create(user.UserId, token.Id);

            await _refreshTokenRepository.AddAsync(newRefreshToken);

            await _refreshTokenRepository.RevokeAsync(token.Id);

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
