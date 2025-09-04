using AuthModule.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;
using SharedKernel.Interfaces;

namespace AuthModule.Application.Commands.UpdateUserRole
{
    public class UpdateRoleUserEventHandler : INotificationHandler<UpdateUserRoleDomainEvent>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateRoleUserEventHandler> _logger;
        public UpdateRoleUserEventHandler(IAuthUserRepository authUserRepository,
            IAuthUnitOfWork unitOfWork,
            ILogger<UpdateRoleUserEventHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(UpdateUserRoleDomainEvent notification, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(notification.UserId, cancellationToken, false);
            user.UpdateRole(notification.NewRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Auth Module] User with ID {UserId} role updated to {NewRole} successfully.", notification.UserId, notification.NewRole);
        }
    }
}
