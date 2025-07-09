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
                .Include(u => u.PhoneNumbers)
                .ToListAsync();

            if (users == null || !users.Any())
            {
                _logger.LogInformation("No users found.");
                return Enumerable.Empty<User>();
            }

            _logger.LogInformation($"Retrieved {users.Count} users from the repository.");
            return users;
        }

        public async Task<User> GetByIdAsync(Guid userId, bool includeDeleted = false)
        {
            var user = await _context.Users
                .Include(u => u.PhoneNumbers)
                .FirstOrDefaultAsync(u => u.Id == userId && (includeDeleted || !u.IsDeleted));
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
            _logger.LogInformation($"User with ID {user.Id} added successfully.");
        }

        public async Task SoftDeleteAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            user.MarkAsDeleted();
            _logger.LogInformation($"User with ID {userId} marked as deleted successfully.");
        }

        public async Task HardDeleteAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId, includeDeleted: true);
            _context.Users.Remove(user);
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            var exists = await _context.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
            _logger.LogInformation($"User with ID {userId} exists: {exists}");
            return exists;
        }

        public async Task<IEnumerable<string>> GetPhoneNumbersAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            var phones = user.GetPhoneNumbers().Select(p => p.PhoneNumber.Value);
            return phones;
        }
    }
}
