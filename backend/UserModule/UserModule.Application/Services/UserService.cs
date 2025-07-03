using UserModule.Application.Dtos.Requests;
using UserModule.Application.Dtos.Responses;
using UserModule.Application.Interfaces;
using UserModule.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using UserModule.Domain.Entities;
using UserModule.Domain.Exceptions;

namespace UserModule.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepository,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserResponse> GetProfile(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found.");
                throw new NullableUserException($"User with ID {userId} not found.");
            }
            var phoneNumbers = user.GetPhoneNumbers();
            var response = new UserResponse
            {
                Name = user.Name,
                Location = user.Location,
                IsDeleted = user.IsDeleted,
                PhoneNumbers = user.GetPhoneNumbers()
                    .Select(p => p.PhoneNumber.Value)
                    .ToList()
            };
            _logger.LogInformation($"Profile retrieved for user {user.Name} with ID {userId}.");
            return response;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                _logger.LogInformation("No users found.");
                return Enumerable.Empty<UserResponse>();
            }
            var userResponses = users.Select(user => new UserResponse
            {
                Name = user.Name,
                Location = user.Location,
                IsDeleted = user.IsDeleted,
                PhoneNumbers = user.GetPhoneNumbers()
                    .Select(p => p.PhoneNumber.Value)
                    .ToList()
            }).ToList();
            _logger.LogInformation($"Retrieved {userResponses.Count} users from the repository.");
            return userResponses;
        }

        public async Task CreateNewProfile(Guid userId, CreateUserRequest request)
        {
            if (request == null)
            {
                _logger.LogError("CreateUserRequest cannot be null.");
                throw new ArgumentNullException(nameof(request), "CreateUserRequest cannot be null.");
            }
            var user = User.Create(userId, request.Name, request.Location);
            foreach (var phone in request.PhoneNumbers)
            {
                user.AddPhoneNumber(phone);
            }
            await _userRepository.AddAsync(user);
            _logger.LogInformation($"New profile created for user {user.Name} with ID {userId}.");
        }

        public async Task UpdateProfile(Guid userId, UpdateUserRequest request)
        {

        }

        public async Task SoftDeleteProfile(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found for soft delete.");
                throw new NullableUserException($"User with ID {userId} not found.");
            }
            await _userRepository.SoftDeleteAsync(userId);
            _logger.LogInformation($"Profile for user {user.Name} with ID {userId} has been soft deleted.");
        }

        public async Task HardDeleteProfile(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found for hard delete.");
                throw new NullableUserException($"User with ID {userId} not found.");
            }
            await _userRepository.HardDeleteAsync(userId);
            _logger.LogInformation($"Profile for user {user.Name} with ID {userId} has been hard deleted.");
        }

        public async Task BlockUser(Guid userId, Guid blockedUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found for blocking.");
                throw new NullableUserException($"User with ID {userId} not found.");
            }
            await _userRepository.BlockUserAsync(userId, blockedUserId);
            _logger.LogInformation($"User with ID {blockedUserId} has been blocked by user {user.Name} with ID {userId}.");
        }

        public async Task UnblockUser(Guid userId, Guid blockedUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found for unblocking.");
                throw new NullableUserException($"User with ID {userId} not found.");
            }
            await _userRepository.UnblockUserAsync(userId, blockedUserId);
            _logger.LogInformation($"User with ID {blockedUserId} has been unblocked by user {user.Name} with ID {userId}.");
        }

        public async Task<IEnumerable<UserResponse>> GetBlockedUsers(Guid userId)
        {
            var blokedUsers = await _userRepository.GetBlockedUsersAsync(userId);
            if (blokedUsers == null || !blokedUsers.Any())
            {
                _logger.LogInformation($"No blocked users found for user with ID {userId}.");
                return Enumerable.Empty<UserResponse>();
            }
            var blockedUserResponses = blokedUsers.Select(user => new UserResponse
            {
                Name = user.Name,
                Location = user.Location,
                IsDeleted = user.IsDeleted,
                PhoneNumbers = user.GetPhoneNumbers()
                    .Select(p => p.PhoneNumber.Value)
                    .ToList()
            }).ToList();
            _logger.LogInformation($"Retrieved {blockedUserResponses.Count} blocked users for user with ID {userId}.");
            return blockedUserResponses;
        }

        public async Task AddPhoneNumber(Guid userId, string phone)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found for adding phone number.");
                throw new NullableUserException($"User with ID {userId} not found.");
            }
            if (await _userRepository.PhoneNumberExistsAsync(phone))
            {
                _logger.LogError($"Phone number {phone} already exists.");
                throw new PhoneNumberIsAlreadyAddedException($"Phone number {phone} already exists.");
            }
            user.AddPhoneNumber(phone);
        }

        public async Task RemovePhoneNumber(Guid userId, int phoneId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found for removing phone number.");
                throw new NullableUserException($"User with ID {userId} not found.");
            }
            user.RemovePhoneNumber(phoneId);
            _logger.LogInformation($"Phone number with ID {phoneId} has been removed from user {user.Name} with ID {userId}.");
        }
    }
}
