using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.User.Commands.DeactivateUser
{
    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeactivateUserCommandHandler> _logger;
        public DeactivateUserCommandHandler(IUserRepository userRepository,
            IUserUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<DeactivateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null || userId.Value != command.UserId)
            {
                _logger.LogWarning("[User Module] User ID is empty or does not match the command. Cannot soft delete user.");
                throw new UnauthorizedAccessException("User is not authenticated or not authorized to delete this profile.");
            }
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken, true, true);
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (user.IsDeleted)
                {
                    _logger.LogWarning("[User Module] User with ID {userId} is already marked as deleted.", command.UserId);
                    return;
                }
                user.MarkAsDeleted();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            _logger.LogInformation("[User Module] Profile for user with ID {userId} has been soft deleted.", command.UserId);
        }
    }
}
