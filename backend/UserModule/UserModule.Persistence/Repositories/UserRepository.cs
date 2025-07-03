using UserModule.Domain.Entities;
using UserModule.Domain.Interfaces;
using UserModule.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace UserModule.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(UserDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();
            _logger.LogInformation($"Retrieved {users.Count} users from the repository.");
            return users;
        }

        public async Task<User> GetByIdAsync(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found.");
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            _logger.LogInformation($"User with ID {userId} retrieved successfully.");
            return user;
        }

        public async Task AddAsync(User user)
        {
            if (await ExistsAsync(user.Id))
            {
                _logger.LogError($"User with ID {user.Id} already exists in the repository.");
                throw new UserExistException($"User with ID {user.Id} already exists.");
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User with ID {user.Id} added successfully.");
        }

        public async Task UpdateAsync(User user)
        {
            if (user == null)
            {
                _logger.LogError("User cannot be null.");
                throw new NullableUserException();
            }

            var exists = await _context.Users.AnyAsync(u => u.Id == user.Id);
            if (!exists)
            {
                _logger.LogError($"User with ID {user.Id} not found.");
                throw new UserNotFoundException($"User with ID {user.Id} not found.");
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User with ID {user.Id} updated successfully.");
        }

        public async Task SoftDeleteAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found for deletion.");
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            user.MarkAsDeleted();
            await UpdateAsync(user);
            _logger.LogInformation($"User with ID {userId} marked as deleted successfully.");
        }

        public async Task HardDeleteAsync(Guid userId)
        {
            var delResult = await _context.Users
                .Where(u => u.Id == userId && !u.IsDeleted)
                .ExecuteDeleteAsync();

            if (delResult == 0)
            {
                _logger.LogError($"User with ID {userId} not found for hard deletion.");
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }

            _logger.LogInformation($"User with ID {userId} hard deleted successfully.");
        }

        public async Task<IEnumerable<User>> GetBlockedUsersAsync(Guid userId)
        {
            var userExist = await _context.Users
                .AnyAsync(u => u.Id == userId && !u.IsDeleted);

            if (!userExist)
            {
                _logger.LogError($"User with ID {userId} not found for retrieving blocked users.");
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }

            var blockedUsers = await _context.UserBlocks
                .Where(b => b.UserId == userId && b.UnblockedAt == null)
                .Join(_context.Users,
                    blockedId => blockedId.BlockedUserId,
                    user => user.Id,
                    (blockedId, user) => user)
                .ToListAsync();
            return blockedUsers;
        }

        public async Task BlockUserAsync(Guid userId, Guid blockedUserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found for blocking.");
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            user.BlockUser(blockedUserId);
            await UpdateAsync(user);
            _logger.LogInformation($"User with ID {blockedUserId} blocked successfully by user with ID {userId}.");
        }

        public async Task UnblockUserAsync(Guid userId, Guid blockedUserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found for unblocking.");
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            user.UnblockUser(blockedUserId);
            await UpdateAsync(user);
            _logger.LogInformation($"User with ID {blockedUserId} unblocked successfully by user with ID {userId}.");
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            var exists = await _context.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
            _logger.LogInformation($"User with ID {userId} exists: {exists}");
            return exists;
        }

        public async Task<IEnumerable<UserPhoneNumber>> GetUserPhoneNumbersAsync(Guid userId)
        {
            var phoneNumbers = await _context.UserPhoneNumbers
                .Where(u => u.UserId == userId)
                .ToListAsync();
            if (phoneNumbers.Count() == 0)
            {
                _logger.LogError($"No phone numbers found for user with ID {userId}.");
                throw new PhoneNumberNotFoundException($"No phone numbers found for user with ID {userId}.");
            }

            _logger.LogInformation($"Retrieved {phoneNumbers.Count()} phone numbers for user with ID {userId}.");
            return phoneNumbers;
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.LogError("Phone number cannot be null or empty.");
                throw new NullablePhoneNumberException("Phone number cannot be null or empty.");
            }
            var exists = await _context.Users
                .AnyAsync(u => u.PhoneNumbers.Any(p => p.PhoneNumber.Value == phoneNumber) && !u.IsDeleted);
            _logger.LogInformation($"Phone number {phoneNumber} exists: {exists}");
            return exists;
        }
    }
}
