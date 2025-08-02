using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using UserModule.Domain.Interfaces;

namespace UserModule.Application.Services
{
    public class UserRestorer : IUserRestorer
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<UserRestorer> _logger;
        public UserRestorer(IUserRepository userRepository,
            IUserUnitOfWork unitOfWork,
            ILogger<UserRestorer> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task RestoreUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken, true);
            user.Restore();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"User with ID {userId} restored successfully.");
        }
    }
}
