using MediatR;
using Microsoft.Extensions.Logging;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.User.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteUserCommandHandler> _logger;
        public DeleteUserCommandHandler(IUserRepository userRepository,
            IUserUnitOfWork userUnitOfWork,
            ILogger<DeleteUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = userUnitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            user.Delete();
            await _userRepository.HardDeleteAsync(command.UserId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[User Module(DeleteUserCommandHandler)] Profile for user with ID {UserId} has been hard deleted.", command.UserId);
        }
    }
}
