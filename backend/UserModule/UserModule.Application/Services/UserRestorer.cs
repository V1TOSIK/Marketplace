using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using UserModule.Domain.Exceptions;
using UserModule.Domain.Interfaces;

namespace UserModule.Application.Services
{
    public class UserRestorer : IUserRestorer
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserRestorer> _logger;
        public UserRestorer(IUserRepository userRepository, ILogger<UserRestorer> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task RestoreUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId, true);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found for restoration.");
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            user.Restore();
            _logger.LogInformation($"User with ID {userId} restored successfully.");
        }
    }
}
