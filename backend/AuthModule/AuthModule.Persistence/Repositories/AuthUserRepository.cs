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


        public async Task<AuthUser> GetByIdAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false, bool includeBanned = false)
        {
            var user = await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.Id == userId
            && (includeDeleted || !u.IsDeleted)
            && (includeBanned || !u.IsBanned), cancellationToken);
            if (user == null)
                throw new UserNotFoundException($"User with Id: {userId} not found");

            return user;
        }

        public async Task<AuthUser?> GetByProviderAsync(string providerUserId, string providerText, CancellationToken cancellationToken, bool includeDeleted = false, bool includeBanned = false)
        {
            var provider = ParseProvider(providerText);
            var user = await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.ProviderUserId == providerUserId
            && u.Provider == provider
            && (includeDeleted || !u.IsDeleted)
            && (includeBanned || !u.IsBanned), cancellationToken);
            if (user == null)
                throw new UserNotFoundException($"User with providerId: {providerUserId} not found");

            return user;
        }

        public async Task<AuthUser?> GetByEmailAsync(string email, CancellationToken cancellationToken, bool includeDeleted = false, bool includeBanned = false)
        {
            var emailValue = new Email(email);

            var user = await _dbContext.AuthUsers
                .FirstOrDefaultAsync(u => u.Email != null
                    && u.Email.Equals(emailValue)
                    && (includeDeleted || !u.IsDeleted)
                    && (includeBanned || !u.IsBanned), cancellationToken);

            return user;
        }

        public async Task<AuthUser?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken, bool includeDeleted = false, bool includeBanned = false)
        {
            var phoneNumberValue = new PhoneNumber(phoneNumber);

            var user = await _dbContext.AuthUsers
                .FirstOrDefaultAsync(u => u.PhoneNumber != null
                    && u.PhoneNumber.Equals(phoneNumberValue)
                    && (includeDeleted || !u.IsDeleted)
                    && (includeBanned || !u.IsBanned), cancellationToken);

            return user;
        }

        public async Task AddAsync(AuthUser user, CancellationToken cancellationToken)
        {
            if (user.IsOAuth())
            {
                if (string.IsNullOrWhiteSpace(user.ProviderUserId))
                    throw new InvalidProviderException("Provider user id cannot be null or empty");

                if (await IsOAuthRegisteredAsync(user.ProviderUserId, user.Provider.ToString(), cancellationToken))
                    throw new OAuthUserAlreadyExistsException($"User with provider user id {user.ProviderUserId} already exists.");
            }
            else
            {
                if (user.Email is not null && await IsEmailRegisteredAsync(user.Email, cancellationToken))
                    throw new EmailAlreadyExistsException($"User with email {user.Email.Value} already exists.");

                if (user.PhoneNumber is not null && await IsPhoneNumberRegisteredAsync(user.PhoneNumber, cancellationToken))
                    throw new PhoneNumberAlreadyExistsException($"User with phone number {user.PhoneNumber.Value} already exists.");
            }

            await _dbContext.AuthUsers.AddAsync(user, cancellationToken);
            _logger.LogInformation($"{(user.IsOAuth() ? "OAuth" : "Local")} user with ID {user.Id} added successfully.");
        }

        public async Task HardDeleteAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(userId, cancellationToken, true, true);
            _dbContext.AuthUsers.Remove(user);

            _logger.LogInformation($"{user.Id} was deleted");
        }

        public async Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken)
        {
            var emailValue = new Email(email);

            return await _dbContext.AuthUsers
                .AnyAsync(u => u.Email != null && u.Email.Equals(emailValue) && !u.IsDeleted && !u.IsBanned, cancellationToken);
        }

        public async Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            var phoneNumberValue = new PhoneNumber(phoneNumber);

            return await _dbContext.AuthUsers
                .AnyAsync(u => u.PhoneNumber != null && u.PhoneNumber.Equals(phoneNumberValue) && !u.IsDeleted && !u.IsBanned, cancellationToken);
        }

        public async Task<bool> IsOAuthRegisteredAsync(string providerUserId, string providerText, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(providerUserId))
                throw new InvalidProviderException("Provider user id cannot be null or empty");

            var provider = ParseProvider(providerText);

            return await _dbContext.AuthUsers
                .AnyAsync(u => u.ProviderUserId == providerUserId && u.Provider == provider && !u.IsDeleted && !u.IsBanned, cancellationToken);
        }

        public async Task<bool> IsExistsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.AuthUsers
                .AnyAsync(u => u.Id == userId && !u.IsDeleted && !u.IsBanned, cancellationToken);
        }

        private AuthProvider ParseProvider(string providerText)
        {
            if (!Enum.TryParse<AuthProvider>(providerText, true, out AuthProvider provider))
                throw new InvalidProviderException("Invalid provider name");
            return provider;
        }
    }
}