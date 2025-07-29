using AuthModule.Application.Exceptions;
using AuthModule.Domain.Entities;
using AuthModule.Domain.Exceptions;
using AuthModule.Domain.Interfaces;
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
        

        public async Task<AuthUser> GetByIdAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false)
        {
            var user = await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.Id == userId && (includeDeleted || !u.IsDeleted), cancellationToken);
            if (user == null)
                throw new UserNotFoundException($"User with Id: {userId} not found");

            return user;
        }

        public async Task<AuthUser?> GetByEmailAsync(string email, CancellationToken cancellationToken, bool includeDeleted = false)
        {
            var emailValue = new Email(email);
            
            var user = await _dbContext.AuthUsers
                .Where(u => u.Email != null
                    && u.Email.Equals(emailValue)
                    && (includeDeleted || !u.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);

            return user;
        }

        public async Task<AuthUser?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken, bool includeDeleted = false)
        {
            var phoneNumberValue = new PhoneNumber(phoneNumber);

            var user = await _dbContext.AuthUsers
                .Where(u => u.PhoneNumber != null
                    && u.PhoneNumber.Equals(phoneNumberValue)
                    && (includeDeleted || !u.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);

            return user;
        }

        public async Task AddAsync(AuthUser user, CancellationToken cancellationToken)
        {
            if (user.Email is not null)
            {
                var emailExist = await IsEmailRegisteredAsync(user.Email, cancellationToken);
                if (emailExist)
                    throw new EmailAlreadyExistsException($"User with email {user.Email.Value} already exists.");
            }

            if (user.PhoneNumber is not null)
            {
                var phoneExist = await IsPhoneNumberRegisteredAsync(user.PhoneNumber, cancellationToken);
                if (phoneExist)
                    throw new PhoneNumberAlreadyExistsException($"User with phone number {user.PhoneNumber.Value} already exists.");
            }

            await _dbContext.AuthUsers.AddAsync(user, cancellationToken);
            _logger.LogInformation($"User with ID {user.Id} added successfully.");
        }

        public async Task HardDeleteAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(userId, cancellationToken);
            _dbContext.AuthUsers.Remove(user);

            _logger.LogInformation($"{user.Id} was deleted");
        }

        public async Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken)
        {
            var emailValue = new Email(email);

            return await _dbContext.AuthUsers
                .AnyAsync(u => u.Email != null && u.Email.Equals(emailValue) && u.CanLogin(), cancellationToken);
        }

        public async Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            var phoneNumberValue = new PhoneNumber(phoneNumber);

            return await _dbContext.AuthUsers
                .AnyAsync(u => u.PhoneNumber != null && u.PhoneNumber.Equals(phoneNumberValue) && u.CanLogin(), cancellationToken);
        }

        public async Task<bool> IsExistsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.AuthUsers
                .AnyAsync(u => u.Id == userId && u.CanLogin(), cancellationToken);
        }

        public async Task<bool> IsExistsAsync(string email, string phoneNumber, CancellationToken cancellationToken)
        {
            var emailValue = new Email(email);
            var phoneNumberValue = new PhoneNumber(phoneNumber);

            return await _dbContext.AuthUsers
                .AnyAsync(u =>
                    (
                        (u.Email != null && u.Email.Equals(emailValue))
                        || (u.PhoneNumber != null && u.PhoneNumber.Equals(phoneNumberValue))
                    )
                    && u.CanLogin(), cancellationToken
                );
        }
    }
}
