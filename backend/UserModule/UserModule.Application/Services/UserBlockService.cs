using Microsoft.Extensions.Logging;
using UserModule.Application.Dtos.Responses;
using UserModule.Domain.Entities;
using UserModule.Domain.Exceptions;
using UserModule.Application.Interfaces.Services;
using UserModule.Application.Interfaces.Repositories;
using UserModule.Application.Interfaces;

namespace UserModule.Application.Services
{
    public class UserBlockService : IUserBlockService
    {
        private readonly IUserBlockRepository _userBlockRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<UserBlockService> _logger;
        public UserBlockService(IUserBlockRepository userBlockRepository,
            IUserUnitOfWork unitOfWork,
            ILogger<UserBlockService> logger)
        {
            _userBlockRepository = userBlockRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task BlockUser(Guid userId, Guid blockedUserId, CancellationToken cancellationToken)
        {
            if (blockedUserId == userId)
            {
                _logger.LogError("User cannot block themselves.");
                throw new InvalidBlockDataException("User cannot block themselves.");
            }
            var block = UserBlock.Create(userId, blockedUserId);
            await _userBlockRepository.AddAsync(block, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UnblockUser(Guid userId, Guid blockedUserId, CancellationToken cancellationToken)
        {
            if (blockedUserId == userId)
            {
                _logger.LogError("User cannot unblock themselves.");
                throw new InvalidBlockDataException("User cannot unblock themselves.");
            }
            var block = await _userBlockRepository.GetActiveBlockAsync(userId, blockedUserId, cancellationToken);
            block.Unblock();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<BlockedUserResponse>> GetBlockedUsers(Guid userId, CancellationToken cancellationToken)
        {
            var blockedUsers = await _userBlockRepository.GetBlockedUsersAsync(userId, cancellationToken);

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
