using UserModule.Domain.Entities;
using UserModule.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UserModule.Application.Interfaces.Repositories;

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
        }

        public async Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false, bool includeBanned = false)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId
                && (includeDeleted || !u.IsDeleted)
                && (includeBanned || !u.IsBanned), cancellationToken);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found.");
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            _logger.LogInformation($"User with ID {userId} retrieved successfully.");
            return user;
        }

        public async Task<User> GetByIdWithPhoneNumbersAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false, bool includeBanned = false)
        {
            var user = await _dbContext.Users
                .Include(u => u.PhoneNumbers)
                .FirstOrDefaultAsync(u => u.Id == userId
                && (includeDeleted || !u.IsDeleted)
                && (includeBanned || !u.IsBanned), cancellationToken);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found.");
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            _logger.LogInformation($"User with ID {userId} retrieved successfully.");
            return user;
        }

        public async Task<IEnumerable<string>> GetUserPhoneNumbersAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.UserPhoneNumbers
                .Where(pn => pn.UserId == userId)
                .Select(pn => pn.PhoneNumber.Value)
                .ToListAsync();
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            if (await ExistsAsync(user.Id, cancellationToken))
            {
                _logger.LogError($"User with ID {user.Id} already exists in the repository.");
                throw new UserExistException($"User with ID {user.Id} already exists.");
            }

            await _dbContext.Users.AddAsync(user, cancellationToken);
            _logger.LogInformation($"User with ID {user.Id} added successfully.");
        }

        public async Task HardDeleteAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(userId, cancellationToken, true, true);
            _dbContext.Users.Remove(user);
        }

        private async Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == userId, cancellationToken);
        }
    }
}
