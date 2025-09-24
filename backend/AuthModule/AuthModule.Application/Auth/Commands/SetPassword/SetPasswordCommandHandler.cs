using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using MediatR;

namespace AuthModule.Application.Auth.Commands.SetPassword
{
    public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<SetPasswordCommandHandler> _logger;
        public SetPasswordCommandHandler(IAuthUserRepository authUserRepository,
            IAuthUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ILogger<SetPasswordCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task Handle(SetPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(command.UserId);

            if (user.Password != null)
            {
                _logger.LogWarning("[Auth Module(SetPasswordCommandHandler)] SetPasswordCommandHandler: User with ID {UserId} already has a password set.", command.UserId);
                throw new InvalidOperationException("Password is already set.");
            }

            var hashedPassword = _passwordHasher.HashPassword(command.Request.Password);

            user.SetPassword(hashedPassword);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("[Auth Module(SetPasswordCommandHandler)]SetPasswordCommandHandler: Password set successfully for user with ID {UserId}.", command.UserId);
        }
    }
}
