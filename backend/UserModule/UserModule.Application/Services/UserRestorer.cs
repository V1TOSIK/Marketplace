using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using UserModule.Domain.Interfaces;

namespace UserModule.Application.Services
{
    public class UserRestorer : IUserRestorer
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserRestorer> _logger;
        public UserRestorer(IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<UserRestorer> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task RestoreUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId, true);
            user.Restore();
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"User with ID {userId} restored successfully.");
        }
    }
}
