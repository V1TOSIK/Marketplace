using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;

namespace AuthModule.Application.Commands.BanUser
{
    public class BanUserEventHandler : INotificationHandler<BanUserDomainEvent>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<BanUserEventHandler> _logger;
        public BanUserEventHandler(IAuthUserRepository authUserRepository,
            IAuthUnitOfWork authUnitOfWork,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<BanUserEventHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _unitOfWork = authUnitOfWork;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }
        public async Task Handle(BanUserDomainEvent notification, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(notification.UserId, cancellationToken, false);
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                user.Ban();
                await _refreshTokenRepository.RevokeAllAsync(notification.UserId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            _logger.LogInformation("[Auth Module] User with ID {UserId} banned successfully.", notification.UserId);
        }
    }
}
