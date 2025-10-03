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

        public async Task<AuthUser?> GetByOAuthAsync(string providerUserId, string providerText, CancellationToken cancellationToken = default)
        {
            var provider = ParseProvider(providerText);
            var oAuthUser = await _dbContext.ExternalLogins
                .FirstOrDefaultAsync(u => u.ProviderUserId == providerUserId
                    && u.Provider == provider, cancellationToken);

            if (oAuthUser == null)
                throw new UserNotFoundException($"OAuthUser with providerUserId {providerUserId} and Provider {provider} not found");

            var user = await _dbContext.AuthUsers
                .Include(u => u.ExternalLogins)
                .FirstOrDefaultAsync(u => u.Id == oAuthUser.UserId, cancellationToken);

            return user;
        }

        public async Task<AuthUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var emailValue = new Email(email);

            var user = await _dbContext.AuthUsers
                .FirstOrDefaultAsync(u => u.Email != null
                    && u.Email.Equals(emailValue),
                    cancellationToken);

            return user;
        }

        public async Task<AuthUser?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            var phoneNumberValue = new PhoneNumber(phoneNumber);

            var user = await _dbContext.AuthUsers
                .FirstOrDefaultAsync(u => u.PhoneNumber != null
                    && u.PhoneNumber.Equals(phoneNumberValue),
                    cancellationToken);

            return user;
        }

        public async Task<AuthUser?> GetByCredentialAsync(string credential, CancellationToken cancellationToken = default)
        {
            if (credential.Contains("@"))
            {
                return await GetByEmailAsync(credential, cancellationToken);
            }
            else
            {
                return await GetByPhoneNumberAsync(credential, cancellationToken);
            }
        }

        public async Task AddAsync(AuthUser user, CancellationToken cancellationToken = default)
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

            await _dbContext.AuthUsers.AddAsync(user, cancellationToken);
        }

        public async Task<IEnumerable<Guid>> GetIdsForDeleteAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.AuthUsers
                .Where(u => u.IsDeleted && u.DeletedAt <= DateTime.UtcNow.AddDays(-7))
                .Select(u => u.Id)
                .ToListAsync(cancellationToken);
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