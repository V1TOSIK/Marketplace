using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.SharedKernel.Interfaces;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;
using UserModule.Domain.Entities;

namespace UserModule.Application.User.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IUserRepository userRepository,
            IUserUnitOfWork UnitOfWork,
            ICurrentUserService CurrentUserService,
            ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = UnitOfWork;
            _currentUserService = CurrentUserService;
            _logger = logger;
        }

        public async Task Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                _logger.LogWarning("[User Module] User ID is empty. Cannot create user profile.");
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var user = UserModule.Domain.Entities.User.Create(userId.Value, command.Name, command.Location);
            foreach (var phone in command.PhoneNumbers)
            {
                user.AddPhoneNumber(phone);
            }
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[User Module] New profile created for user {Name} with ID {UserId}.", user.Name, userId.Value);
        }
    }
}
