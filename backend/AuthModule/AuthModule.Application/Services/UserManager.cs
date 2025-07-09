using AuthModule.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;

namespace AuthModule.Application.Services
{
    public class UserManager : IUserManager
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserManager> _logger;
        public UserManager(IAuthUserRepository authUserRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            ILogger<UserManager> logger)
        {
            _authUserRepository = authUserRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task HardDeleteUser(Guid userId)
        {
            await _authUserRepository.HardDeleteUserAsync(userId);
            await _refreshTokenRepository.RevokeAllAsync(userId);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"User with ID {userId} hard deleted successfully.");
        }

        public async Task SoftDeleteUser(Guid userId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _authUserRepository.SoftDeleteUserAsync(userId);
                await _refreshTokenRepository.RevokeAllAsync(userId);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"User with ID {userId} soft deleted successfully.");
            });
        }
    }
}
