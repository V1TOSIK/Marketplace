using AuthModule.Application.Exceptions;
using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Domain.Entities;
using AuthModule.Domain.Enums;
using AuthModule.Domain.Exceptions;
using AuthModule.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.ValueObjects;

namespace AuthModule.Persistence.Repositories
{
    public class AuthUserRepository : IAuthUserRepository
    {
        private readonly AuthDbContext _dbContext;
        private readonly ILogger<AuthUserRepository> _logger;
        public AuthUserRepository(AuthDbContext dbContext,
            ILogger<AuthUserRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        public async Task<AuthUser> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
            {
                _logger.LogError("[Auth Module(Repository)] User with Id: {userId} not found", userId);
                throw new UserNotFoundException($"User with Id: {userId} not found");
            }

            return user;
        }

        public async Task<AuthUser?> GetByOAuthAsync(string idToken, string providerText, CancellationToken cancellationToken = default)
        {
            var provider = ParseProvider(providerText);
            var user = await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.ProviderUserId == idToken
            && u.Provider == provider,
            cancellationToken);

            return user;
        }

        public async Task<AuthUser?> GetByEmailAsync(string email, bool includeDeleted = false, bool includeBanned = false, CancellationToken cancellationToken = default)
        {
            var emailValue = new Email(email);

            var user = await _dbContext.AuthUsers
                .FirstOrDefaultAsync(u => u.Email != null
                    && u.Email.Equals(emailValue)
                    && (includeDeleted || !u.IsDeleted)
                    && (includeBanned || !u.IsBanned), cancellationToken);

            return user;
        }

        public async Task<AuthUser?> GetByPhoneNumberAsync(string phoneNumber,bool includeDeleted = false, bool includeBanned = false, CancellationToken cancellationToken = default)
        {
            var phoneNumberValue = new PhoneNumber(phoneNumber);

            var user = await _dbContext.AuthUsers
                .FirstOrDefaultAsync(u => u.PhoneNumber != null
                    && u.PhoneNumber.Equals(phoneNumberValue)
                    && (includeDeleted || !u.IsDeleted)
                    && (includeBanned || !u.IsBanned), cancellationToken);

            return user;
        }

        public async Task<AuthUser?> GetByCredentialAsync(string credential, bool includeDeleted = false, bool includeBanned = false, CancellationToken cancellationToken = default)
        {
            if (credential.Contains("@"))
            {
                return await GetByEmailAsync(credential, includeDeleted, includeBanned, cancellationToken);
            }
            else
            {
                return await GetByPhoneNumberAsync(credential, includeDeleted, includeBanned, cancellationToken);
            }
        }

        public async Task AddAsync(AuthUser user, CancellationToken cancellationToken = default)
        {
            if (user.IsOAuth())
            {
                if (string.IsNullOrWhiteSpace(user.ProviderUserId))
                {
                    _logger.LogError("[Auth Module(Repository)] Provider user id cannot be null or empty");
                    throw new InvalidProviderException("Provider user id cannot be null or empty");
                }

                if (await IsOAuthRegisteredAsync(user.ProviderUserId, user.Provider.ToString(), cancellationToken))
                {
                    _logger.LogError("[Auth Module(Repository)] User with provider user id {providerUserId} already exists", user.ProviderUserId);
                    throw new OAuthUserAlreadyExistsException($"User with provider user id {user.ProviderUserId} already exists.");
                }
            }
            else
            {
                if (user.Email is not null && await IsEmailRegisteredAsync(user.Email, cancellationToken))
                {
                    _logger.LogError("[Auth Module(Repository)] User with email {email} already exists", user.Email.Value);
                    throw new EmailAlreadyExistsException($"User with email {user.Email.Value} already exists.");
                }

                if (user.PhoneNumber is not null && await IsPhoneNumberRegisteredAsync(user.PhoneNumber, cancellationToken))
                {
                    _logger.LogError("[Auth Module(Repository)] User with phone number {phoneNumber} already exists", user.PhoneNumber.Value);
                    throw new PhoneNumberAlreadyExistsException($"User with phone number {user.PhoneNumber.Value} already exists.");
                }
            }

            await _dbContext.AuthUsers.AddAsync(user, cancellationToken);
        }

        public async Task HardDeleteAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await GetByIdAsync(userId, cancellationToken);
            _dbContext.AuthUsers.Remove(user);
        }

        public async Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken = default)
        {
            var emailValue = new Email(email);

            return await _dbContext.AuthUsers
                .AnyAsync(u => u.Email != null && u.Email.Equals(emailValue) && !u.IsDeleted && !u.IsBanned, cancellationToken);
        }

        public async Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            var phoneNumberValue = new PhoneNumber(phoneNumber);

            return await _dbContext.AuthUsers
                .AnyAsync(u => u.PhoneNumber != null && u.PhoneNumber.Equals(phoneNumberValue) && !u.IsDeleted && !u.IsBanned, cancellationToken);
        }

        public async Task<bool> IsOAuthRegisteredAsync(string providerUserId, string providerText, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(providerUserId))
            {
                _logger.LogError("[Auth Module(Repository)] Provider user id cannot be null or empty");
                throw new InvalidProviderException("Provider user id cannot be null or empty");
            }

            var provider = ParseProvider(providerText);

            return await _dbContext.AuthUsers
                .AnyAsync(u => u.ProviderUserId == providerUserId && u.Provider == provider && !u.IsDeleted && !u.IsBanned, cancellationToken);
        }

        public async Task<bool> IsExistsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.AuthUsers
                .AnyAsync(u => u.Id == userId && !u.IsDeleted && !u.IsBanned, cancellationToken);
        }

        private AuthProvider ParseProvider(string providerText)
        {
            if (!Enum.TryParse<AuthProvider>(providerText, true, out AuthProvider provider))
            {
                _logger.LogError("[Auth Module(Repository)] Invalid provider name: {providerText}", providerText);
                throw new InvalidProviderException("Invalid provider name");
            }
            return provider;
        }
    }
}