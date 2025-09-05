using MediatR;
using Microsoft.Extensions.Logging;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.User.Commands.BanUser
{
    public class BanUserCommandHandler : IRequestHandler<BanUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<BanUserCommandHandler> _logger;
        public BanUserCommandHandler(IUserRepository userRepository,
            IUserUnitOfWork unitOfWork,
            ILogger<BanUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(BanUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken, false, false);
            user.Ban();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[User Module] User {Name} with ID {UserId} has been banned.", user.Name, command.UserId);
        }
    }
}
