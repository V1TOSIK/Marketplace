using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Interfaces;
using AuthModule.Domain.Entities;
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

        public async Task<AuthorizeResponse> Register(RegisterRequest request)
        {
            AuthUser user;
            var hashPassword = _passwordHasher.HashPassword(request.Password);

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailCheck = await _authUserRepository.IsEmailRegisteredAsync(request.Email);
                if (!emailCheck)
                {
                    user = AuthUser.Create(request.Email, null, hashPassword, request.Role);
                }
                else
                {
                    throw new InvalidOperationException($"Email {request.Email} is already registered.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phoneCheck = await _authUserRepository.IsPhoneNumberRegisteredAsync(request.PhoneNumber);

                if (!phoneCheck)
                {
                    user = AuthUser.Create(null, request.PhoneNumber, hashPassword, request.Role);
                }
                else
                {
                    throw new InvalidOperationException($"Phone number {request.PhoneNumber} is already registered.");
                }
            }
            else
            {
                throw new ArgumentException("Either email or phone number must be provided for login.");

            }

            await _authUserRepository.AddUserAsync(user);

            var refreshToken = RefreshToken.Create(user.UserId);
            await _refreshTokenRepository.AddAsync(refreshToken);

            return new AuthorizeResponse
            {
                UserId = user.UserId,
                Role = user.Role.ToString(),
                RefreshToken = refreshToken.Token
            }; 
        }

        public async Task<AuthorizeResponse> Login(LoginRequest request)
        {
            AuthUser? user = null;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user = await _authUserRepository.GetUserByEmailAsync(request.Email);
                if (user == null)
                    throw new InvalidOperationException($"User with {request.Email} is not registered.");
                
            }
            else if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                user = await _authUserRepository.GetUserByPhoneNumberAsync(request.PhoneNumber);

                if (user == null)
                    throw new InvalidOperationException($"User with {request.PhoneNumber} is not registered.");
            }
            else
            {
                throw new ArgumentException("User with this email and phone number is not registered");
            }

            if (user.IsDeleted)
                throw new InvalidOperationException("User is deleted.");

            if (!_passwordHasher.VerifyHashedPassword(user.Password.Value, request.Password))
                throw new InvalidOperationException("Invalid password.");

            var refreshToken = RefreshToken.Create(user.UserId);
            await _refreshTokenRepository.AddAsync(refreshToken);

            return new AuthorizeResponse
            {
                UserId = user.UserId,
                Role = user.Role.ToString(),

                RefreshToken = refreshToken.Token,
            };
        }

        public async Task LogoutFromAllDevices(Guid userId)
        {
            var userExists = await _authUserRepository.IsUserExistsAsync(userId);
            if (!userExists)
                throw new InvalidOperationException($"User with ID {userId} does not exist.");

            await _refreshTokenRepository.RevokeAllAsync(userId);
        }

        public async Task LogoutFromDevice(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("RefreshToken is not valid");

            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null)
                throw new InvalidOperationException("Refresh token not found or already revoked.");

            await _refreshTokenRepository.RevokeAsync(token.Id);
        }

        public async Task<AuthorizeResponse> RefreshTokens(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("RefreshToken is not valid");

            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            
            if (token == null || token.IsRevoked)
                throw new InvalidOperationException("Refresh token not found or already revoked.");
            
            if (token.ExpirationDate < DateTime.UtcNow)
            {
                await _refreshTokenRepository.RevokeAsync(token.Id);
                throw new InvalidOperationException("Refresh token has expired.");
            }
            
            var user = await _authUserRepository.GetUserByIdAsync(token.UserId);
            
            if (user == null || user.IsDeleted)
                throw new InvalidOperationException("User not found or deleted.");
            
            var newRefreshToken = RefreshToken.Create(user.UserId, token.Id);
            
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            await _refreshTokenRepository.RevokeAsync(token.Id);

            return new AuthorizeResponse
            {
                UserId = user.UserId,
                Role = user.Role.ToString(),
                RefreshToken = newRefreshToken.Token
            };
        }
    }
}
