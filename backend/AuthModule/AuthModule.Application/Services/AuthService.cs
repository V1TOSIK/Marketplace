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

        public async Task<AuthorizeUserResponse> Register(RegisterUserRequest request)
        {
            AuthUser user;
            var hashPassword = _passwordHasher.HashPassword(request.Password);

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailCheck = await _authUserRepository.IsEmailRegisteredAsync(request.Email);
                if (emailCheck)
                {
                    user = await _authUserRepository.GetUserByEmailAsync(request.Email);

                    if (user.IsDeleted)
                    {
                        if (_passwordHasher.VerifyHashedPassword(user.Password.Value, request.Password))
                        {
                            user.Restore();
                            await _authUserRepository.UpdateUserAsync(user);
                        }
                        else
                        {
                            throw new InvalidOperationException("Invalid password for existing user.");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Email {request.Email} is already registered.");
                    }

                }
                else
                {
                    user = AuthUser.Create(request.Email, null, hashPassword, request.Role);
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phoneCheck = await _authUserRepository.IsPhoneNumberRegisteredAsync(request.PhoneNumber);

                if (phoneCheck)
                {
                    user = await _authUserRepository.GetUserByPhoneNumberAsync(request.PhoneNumber);

                    if (user.IsDeleted)
                    {
                        if (_passwordHasher.VerifyHashedPassword(user.Password.Value, request.Password))
                        {
                            user.Restore();
                            await _authUserRepository.UpdateUserAsync(user);
                        }
                        else
                        {
                            throw new InvalidOperationException("Invalid password for existing user.");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Phone number {request.PhoneNumber} is already registered.");
                    }
                }
                else
                {
                    user = AuthUser.Create(null, request.PhoneNumber, hashPassword, request.Role);
                }
            }
            else
            {
                throw new ArgumentException("Either email or phone number must be provided for login.");

            }

            await _authUserRepository.AddUserAsync(user);

            var refreshToken = RefreshToken.Create(user.UserId);
            await _refreshTokenRepository.AddAsync(refreshToken);

            return new AuthorizeUserResponse
            {
                UserId = user.UserId,
                Role = user.Role.ToString(),
                RefreshToken = refreshToken.Token
            }; 
        }

        public async Task<AuthorizeUserResponse> Login(AuthorizeUserRequest request)
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

            return new AuthorizeUserResponse
            {
                UserId = user.UserId,
                Role = user.Role.ToString(),

                RefreshToken = refreshToken.Token,
            };
        }

        public async Task<bool> Logout(Guid userId)
        {
            var userExists = await _authUserRepository.IsUserExistsAsync(userId);
            if (!userExists)
                throw new InvalidOperationException($"User with ID {userId} does not exist.");

            await _refreshTokenRepository.RevokeAllAsync(userId);

            return true;
        }
    }
}
