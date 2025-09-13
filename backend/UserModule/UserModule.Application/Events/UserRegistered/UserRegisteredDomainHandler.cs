using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.Events.UserRegistered
{
    public class UserRegisteredDomainHandler : INotificationHandler<UserRegisteredEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<UserRegisteredDomainHandler> _logger;

        public UserRegisteredDomainHandler(IUserRepository userRepository,
            IUserUnitOfWork unitOfWork,
            ILogger<UserRegisteredDomainHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var user = Domain.Entities.User.Create(notification.UserId, "none", "none");
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("User with ID {UserId} has been created in UserModule.", notification.UserId);
        }
    }
}
