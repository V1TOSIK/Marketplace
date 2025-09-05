using UserModule.Domain.Entities;
using UserModule.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UserModule.Application.Interfaces.Repositories;
using UserModule.Application.Dtos;
using System.Security.Cryptography;

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
                _logger.LogError("[User Module(Repository)] User with ID {userId} not found.", userId);
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            return user;
        }

        public async Task<UserDto> GetByIdWithPhoneNumbersAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false, bool includeBanned = false)
        {
            var user = await _dbContext.Users
                .Join(_dbContext.UserPhoneNumbers,
                    u => u.Id,
                    pn => pn.UserId,
                    (u, pn) => new { User = u, PhoneNumber = pn })
                .FirstOrDefaultAsync(u => u.User.Id == userId
                && (includeDeleted || !u.User.IsDeleted)
                && (includeBanned || !u.User.IsBanned), cancellationToken);
            if (user == null)
            {
                _logger.LogError("[User Module(Repository)] User with ID {userId} not found.", userId);
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            // need to fetch phone numbers separately to avoid multiple records due to join
            var userDto = new UserDto()
            {
                Name = user.User.Name,
                Location = user.User.Location,
                PhoneNumbers = []
            };
            return userDto;
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
                _logger.LogError("[User Module(Repository)] User with ID {Id} already exists in the repository.", user.Id);
                throw new UserExistException($"User with ID {user.Id} already exists.");
            }

            await _dbContext.Users.AddAsync(user, cancellationToken);
            _logger.LogInformation("[User Module(Repository)] User with ID {Id} added successfully.", user.Id);
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
