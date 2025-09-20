using MediatR;
using Microsoft.Extensions.Logging;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.User.Commands.UnbanUser
{
    public class UnbanUserCommandHandler : IRequestHandler<UnbanUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<UnbanUserCommandHandler> _logger;

        public UnbanUserCommandHandler(IUserRepository userRepository,
            IUserUnitOfWork userUnitOfWork,
            ILogger<UnbanUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = userUnitOfWork;
            _logger = logger;
        }

        public async Task Handle(UnbanUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            user.UnBan();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[User Module(UnBanUserCommandHandler)] Profile for user with ID {UserId} has been unbanned.", command.UserId);
        }
    }
}
