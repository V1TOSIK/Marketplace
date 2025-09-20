using MediatR;
using Microsoft.Extensions.Logging;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.User.Commands.DeactivateUser
{
    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<DeactivateUserCommandHandler> _logger;
        public DeactivateUserCommandHandler(IUserRepository userRepository,
            IUserUnitOfWork unitOfWork,
            ILogger<DeactivateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            user.MarkAsDeleted();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[User Module(DeactivateUserCommandHandler)] Profile for user with ID {userId} has been soft deleted.", command.UserId);
        }
    }
}
