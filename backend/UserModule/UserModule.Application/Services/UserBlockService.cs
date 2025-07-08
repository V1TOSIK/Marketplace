using Microsoft.Extensions.Logging;
using UserModule.Application.Dtos.Responses;
using UserModule.Application.Interfaces;
using UserModule.Domain.Entities;
using UserModule.Domain.Interfaces;
using UserModule.Domain.Exceptions;
using SharedKernel.Interfaces;

namespace UserModule.Application.Services
{
    public class UserBlockService : IUserBlockService
    {
        private readonly IUserBlockRepository _userBlockRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserBlockService> _logger;
        public UserBlockService(IUserBlockRepository userBlockRepository,
            IUnitOfWork unitOfWork,
            ILogger<UserBlockService> logger)
        {
            _userBlockRepository = userBlockRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task BlockUser(Guid userId, Guid blockedUserId)
        {
            if (blockedUserId == userId)
            {
                _logger.LogError("User cannot block themselves.");
                throw new InvalidBlockDataException("User cannot block themselves.");
            }
            if (await _userBlockRepository.ExistsAsync(userId, blockedUserId))
            {
                _logger.LogError("User already blocked");
                throw new BlockExistException("User already blocked");
            }
            var block = UserBlock.Create(userId, blockedUserId);
            await _userBlockRepository.AddAsync(block);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnblockUser(Guid userId, Guid blockedUserId)
        {
            if (blockedUserId == userId)
            {
                _logger.LogError("User cannot unblock themselves.");
                throw new InvalidBlockDataException("User cannot unblock themselves.");
            }
            if (!await _userBlockRepository.ExistsAsync(userId, blockedUserId))
            {
                _logger.LogError("User is not blocked");
                throw new BlockExistException("User is not blocked");
            }
            var block = await _userBlockRepository.GetAsync(userId, blockedUserId);
            block.Unblock();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<BlockedUserResponse>> GetBlockedUsers(Guid userId)
        {
            var blockedUsers = await _userBlockRepository.GetBlockedUsersAsync(userId);

            if (!blockedUsers.Any())
            {
                _logger.LogInformation($"No blocked users found for user {userId}");
                return Enumerable.Empty<BlockedUserResponse>();
            }

            var response = blockedUsers.Select(u => new BlockedUserResponse()
            {
                UserId = u.Id,
                Name = u.Name,
                Location = u.Location,
                PhoneNumbers = u.GetPhoneNumbersValue().ToList()
            });

            return response;
        }
    }
}
