using AuthModule.Application.Exceptions;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthModule.Application.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;
        public ChangePasswordCommandHandler(IAuthUserRepository authUserRepository,
            IPasswordHasher passwordHasher,
            IAuthUnitOfWork unitOfWork,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(command.UserId, cancellationToken);

            user.EnsureActive();
            if (!_passwordHasher.VerifyHashedPassword(user.Password?.Value ?? string.Empty, command.CurrentPassword))
            {
                _logger.LogWarning("[Auth Module(ChangePasswordCommandHandler)] Invalid password attempt for user with ID {userId}.", user.Id);
                throw new IncorrectCredentialsException("Invalid password.");
            }

            var newHashedPassword = _passwordHasher.HashPassword(command.NewPassword);
            user.UpdatePassword(newHashedPassword);

            _logger.LogInformation("[Auth Module(ChangePasswordCommandHanlder)] Password changed successfully for user with ID {userId}.", user.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
