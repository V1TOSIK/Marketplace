using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;

namespace AuthModule.Application.Auth.Commands.LogoutFromAllDevices
{
    public class LogoutFromAllDevicesCommandHandler : IRequestHandler<LogoutFromAllDevicesCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<LogoutFromAllDevicesCommandHandler> _logger;

        public LogoutFromAllDevicesCommandHandler(IRefreshTokenRepository refreshTokenRepository, 
            ICurrentUserService currentUserService,
            IAuthUnitOfWork unitOfWork,
            ILogger<LogoutFromAllDevicesCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(LogoutFromAllDevicesCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                _logger.LogWarning("[Auth Module] No authenticated user found for logout from all devices.");
                throw new UnauthorizedAccessException("No authenticated user found.");
            }
            await _refreshTokenRepository.RevokeAllAsync(userId.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Auth Module] All devices logged out for user with ID {userId}.", userId);
        }
    }
}
