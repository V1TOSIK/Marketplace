using AuthModule.Domain.Entities;
using AuthModule.Domain.Interfaces;
using AuthModule.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace AuthModule.Persistence.Repositories
{
    public class AuthUserRepository : IAuthUserRepository
    {
        private readonly AuthDbContext _dbContext;
        public AuthUserRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddUserAsync(AuthUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (user.Email is not null)
            {
                var emailExist = await _dbContext.AuthUsers
                    .AnyAsync(u => u.Email!.Equals(user.Email));
                if (emailExist)
                    throw new InvalidOperationException($"User with email {user.Email.Value} already exists.");
            }

            if (user.PhoneNumber is not null)
            {
                var phoneExist = await _dbContext.AuthUsers
                    .AnyAsync(u => u.PhoneNumber!.Equals(user.PhoneNumber));
                if (phoneExist)
                    throw new InvalidOperationException($"User with phone number {user.PhoneNumber.Value} already exists.");
            }

            await _dbContext.AuthUsers.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task HardDeleteUserAsync(Guid userId)
        {
            var deleteResult = await _dbContext.AuthUsers
                .Where(u => u.UserId == userId)
                .ExecuteDeleteAsync();

            if (deleteResult == 0)
                throw new InvalidOperationException($"User with ID {userId} does not exist.");
        }

        public async Task SoftDeleteUserAsync(Guid userId)
        {
            var user = await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} does not exist.");

            user.MarkAsDeleted();
            await _dbContext.SaveChangesAsync();
        }

        public async Task RestoreUserAsync(Guid userId)
        {
            var user = await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} does not exist.");

            user.Restore();
            await _dbContext.SaveChangesAsync();
        }

        public async Task<AuthUser?> GetUserByEmailAsync(string email)
        {
            var user = await _dbContext.AuthUsers
                .Where(u => u.Email != null && u.Email.Equals(new Email(email)) && !u.IsDeleted)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<AuthUser?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            var user = await _dbContext.AuthUsers
                .Where(u => u.PhoneNumber != null && u.PhoneNumber.Equals(phoneNumber) && !u.IsDeleted)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<AuthUser?> GetUserByIdAsync(Guid userId)
        {
            var user = await _dbContext.AuthUsers
                .Where(u => u.UserId == userId && !u.IsDeleted)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            return await _dbContext.AuthUsers
                .Where(u => u.Email != null && u.Email.Equals(new Email(email)) && !u.IsDeleted)
                .AnyAsync();
        }

        public async Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber)
        {
            return await _dbContext.AuthUsers
                .Where(u => u.PhoneNumber != null && u.PhoneNumber.Equals(phoneNumber) && !u.IsDeleted)
                .AnyAsync();
        }

        public async Task<bool> IsUserExistsAsync(Guid userId)
        {
            return await _dbContext.AuthUsers
                .Where(u => u.UserId == userId && !u.IsDeleted)
                .AnyAsync();
        }

        public async Task<bool> IsUserExistsAsync(string email, string phoneNumber)
        {
            return await _dbContext.AuthUsers
                .Where(u => !u.IsDeleted)
                .AnyAsync(u =>
                    (u.Email != null && u.Email.Equals(new Email(email))) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Equals(new PhoneNumber(phoneNumber)))
                );
        }

        public async Task UpdateUserAsync(AuthUser user)
        {
            _dbContext.AuthUsers.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
