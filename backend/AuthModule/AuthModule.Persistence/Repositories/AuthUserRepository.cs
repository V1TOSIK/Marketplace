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
            Console.WriteLine($"[AuthUserRepository] DbContext HashCode: {_dbContext.GetHashCode()}");
        }
        

        public async Task<AuthUser> GetByIdAsync(Guid userId, bool includeDeleted = false)
        {
            var user = await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.Id == userId && (includeDeleted || !u.IsDeleted));
            if (user == null)
                throw new UserNotFoundException($"User with Id: {userId} not found");

            return user;
        }

        public async Task<AuthUser?> GetByEmailAsync(string email, bool includeDeleted = false)
        {
            var emailValue = new Email(email);
            
            var user = await _dbContext.AuthUsers
                .Where(u => u.Email != null
                    && u.Email.Equals(emailValue)
                    && (includeDeleted || !u.IsDeleted))
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<AuthUser?> GetByPhoneNumberAsync(string phoneNumber, bool includeDeleted = false)
        {
            var phoneNumberValue = new PhoneNumber(phoneNumber);

            var user = await _dbContext.AuthUsers
                .Where(u => u.PhoneNumber != null
                    && u.PhoneNumber.Equals(phoneNumberValue)
                    && (includeDeleted || !u.IsDeleted))
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task AddAsync(AuthUser user)
        {
            if (user.Email is not null)
            {
                var emailExist = await IsEmailRegisteredAsync(user.Email);
                if (emailExist)
                    throw new EmailAlreadyExistsException($"User with email {user.Email.Value} already exists.");
            }

            if (user.PhoneNumber is not null)
            {
                var phoneExist = await IsPhoneNumberRegisteredAsync(user.PhoneNumber);
                if (phoneExist)
                    throw new PhoneNumberAlreadyExistsException($"User with phone number {user.PhoneNumber.Value} already exists.");
            }

            await _dbContext.AuthUsers.AddAsync(user);
            _logger.LogInformation("User with ID {UserId} added successfully.", user.Id);
        }

        public async Task HardDeleteAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            _dbContext.AuthUsers.Remove(user);

            _logger.LogInformation($"{user.Id} was deleted");
        }

        public async Task SoftDeleteAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);

            user.MarkAsDeleted();

            _logger.LogInformation("User with ID {UserId} soft deleted successfully.", userId);
        }

        public async Task RestoreAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);

            user.Restore();

            _logger.LogInformation("User with ID {UserId} restored successfully.", userId);
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            var emailValue = new Email(email);

            return await _dbContext.AuthUsers
                .AnyAsync(u => u.Email != null && u.Email.Equals(emailValue) && !u.IsDeleted);
        }

        public async Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber)
        {
            var phoneNumberValue = new PhoneNumber(phoneNumber);

            return await _dbContext.AuthUsers
                .AnyAsync(u => u.PhoneNumber != null && u.PhoneNumber.Equals(phoneNumberValue) && !u.IsDeleted);
        }

        public async Task<bool> IsExistsAsync(Guid userId)
        {
            return await _dbContext.AuthUsers
                .AnyAsync(u => u.Id == userId && !u.IsDeleted);
        }

        public async Task<bool> IsExistsAsync(string email, string phoneNumber)
        {
            var emailValue = new Email(email);
            var phoneNumberValue = new PhoneNumber(phoneNumber);

            return await _dbContext.AuthUsers
                .AnyAsync(u =>
                    (u.Email != null && u.Email.Equals(emailValue)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Equals(phoneNumberValue)) &&
                    !u.IsDeleted
                );
        }
    }
}
