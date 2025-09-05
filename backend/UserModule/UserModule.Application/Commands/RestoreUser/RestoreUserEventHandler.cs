using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.Commands.RestoreUser
{
    public class RestoreUserEventHandler : INotificationHandler<RestoreUserDomainEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<RestoreUserEventHandler> _logger;

        public RestoreUserEventHandler(IUserRepository userRepository,
            IUserUnitOfWork unitOfWork,
            ILogger<RestoreUserEventHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(RestoreUserDomainEvent notification, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken, true, true);
            user.Restore();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[User Module] Profile for user {Name} with ID {UserId} has been restored.", user.Name, notification.UserId);
        }
    }
}
