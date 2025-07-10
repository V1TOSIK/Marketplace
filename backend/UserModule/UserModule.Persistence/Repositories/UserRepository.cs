using UserModule.Domain.Entities;
using UserModule.Domain.Interfaces;
using UserModule.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace UserModule.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(UserDbContext dbContext, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            Console.WriteLine($"[UserRepository] DbContext HashCode: {_dbContext.GetHashCode()}");
        }

        public async Task<User> GetByIdAsync(Guid userId, bool includeDeleted = false)
        {
            var user = await _dbContext.Users
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

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _dbContext.Users
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

        public async Task AddAsync(User user)
        {
            if (await ExistsAsync(user.Id))
            {
                _logger.LogError($"User with ID {user.Id} already exists in the repository.");
                throw new UserExistException($"User with ID {user.Id} already exists.");
            }

            await _dbContext.Users.AddAsync(user);
            _logger.LogInformation($"User with ID {user.Id} added successfully.");
        }

        public async Task HardDeleteAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId, includeDeleted: true);
            _dbContext.Users.Remove(user);
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == userId);
        }
    }
}
