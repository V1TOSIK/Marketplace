using AuthModule.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;
using SharedKernel.Interfaces;

namespace AuthModule.Application.Commands.SoftDeleteUser
{
    public class SoftDeleteUserEventHandler : INotificationHandler<SoftDeleteUserDomainEvent>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<SoftDeleteUserEventHandler> _logger;

        public SoftDeleteUserEventHandler(IAuthUserRepository authUserRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IAuthUnitOfWork authUnitOfWork,
            ILogger<SoftDeleteUserEventHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = authUnitOfWork;
            _logger = logger;
        }

        public async Task Handle(SoftDeleteUserDomainEvent notification, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(notification.UserId, cancellationToken, true, false);
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                user.MarkAsDeleted();
                await _refreshTokenRepository.RevokeAllAsync(notification.UserId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            _logger.LogInformation("[Auth Module] User with ID {UserId} soft deleted successfully.", notification.UserId);
        }
    }
}
