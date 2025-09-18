using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;

namespace AuthModule.Application.Events.UnbanUser
{
    public class UnbanEventUserHandler : INotificationHandler<UnbanUserEvent>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<UnbanEventUserHandler> _logger;
        public UnbanEventUserHandler(IAuthUserRepository authUserRepository,
            IAuthUnitOfWork unitOfWork,
            ILogger<UnbanEventUserHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(UnbanUserEvent notification, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(notification.UserId, cancellationToken);
            user.Unban();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Auth Module] User with ID {UserId} unbanned successfully.", notification.UserId);
        }
    }
}
