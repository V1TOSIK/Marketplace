using AuthModule.Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;

namespace AuthModule.Application.Services
{
    public class UserManager : IUserManager
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<UserManager> _logger;
        public UserManager(IAuthUserRepository authUserRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IAuthUnitOfWork unitOfWork,
            ILogger<UserManager> logger)
        {
            _authUserRepository = authUserRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task HardDeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            await _authUserRepository.HardDeleteAsync(userId, cancellationToken);
            await _refreshTokenRepository.RevokeAllAsync(userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"User with ID {userId} hard deleted successfully.", cancellationToken);
        }

        public async Task SoftDeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(userId, cancellationToken, false);
            user.MarkAsDeleted();
            await _refreshTokenRepository.RevokeAllAsync(userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"User with ID {userId} soft deleted successfully.", cancellationToken);
        }

        public async Task Ban(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(userId, cancellationToken, false);
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _refreshTokenRepository.RevokeAllAsync(userId, cancellationToken);
                user.Ban();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
        }

        public async Task UnBan(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(userId, cancellationToken, false);

            user.Unban();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateRole(Guid userId, string roleText, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(userId, cancellationToken, false);
            user.UpdateRole(roleText);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"User with ID {userId} role updated to {roleText} successfully.",cancellationToken);
        }
    }
}
