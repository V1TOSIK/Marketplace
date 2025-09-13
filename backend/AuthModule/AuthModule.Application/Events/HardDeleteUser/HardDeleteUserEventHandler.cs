using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;

namespace AuthModule.Application.Events.HardDeleteUser
{
    public class HardDeleteUserEventHandler : INotificationHandler<HardDeleteUserEvent>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<HardDeleteUserEventHandler> _logger;

        public HardDeleteUserEventHandler(IAuthUserRepository authUserRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IAuthUnitOfWork unitOfWork,
            ILogger<HardDeleteUserEventHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public Task Handle(HardDeleteUserEvent notification, CancellationToken cancellationToken)
        {
            return _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _refreshTokenRepository.RevokeAllAsync(notification.UserId, cancellationToken);
                await _authUserRepository.HardDeleteAsync(notification.UserId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("[Auth Module] User with ID {UserId} hard deleted successfully.", notification.UserId);
            }, cancellationToken);
        }
    }
}
