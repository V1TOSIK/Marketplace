using AuthModule.Application.Exceptions;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;

namespace AuthModule.Application.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;
        public ChangePasswordCommandHandler(IAuthUserRepository authUserRepository,
            IPasswordHasher passwordHasher,
            IAuthUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (userId == null)
            {
                _logger.LogWarning("[Auth Module] Unauthorized password change attempt.");
                throw new UnauthorizedChangePasswordException("You are not authorized to change this user's password.");
            }

            var user = await _authUserRepository.GetByIdAsync(userId.Value, true, true, cancellationToken);

            user.ThrowIfCannotLogin();
            if (user.IsOAuth())
            {
                _logger.LogWarning("[Auth Module] User with ID {userId} was registered by OAuth and cannot change password.", user.Id);
                throw new OAuthUserCannotChangePasswordException("User was registered by OAuth and have not password");
            }

            if (!_passwordHasher.VerifyHashedPassword(user.Password?.Value ?? string.Empty, command.CurrentPassword))
            {
                _logger.LogWarning("[Auth Module] Invalid password attempt for user with ID {userId}.", user.Id);
                throw new IncorrectCredentialsException("Invalid password.");
            }

            var newHashedPassword = _passwordHasher.HashPassword(command.NewPassword);
            user.UpdatePassword(newHashedPassword);

            _logger.LogInformation("[Auth Module] Password changed successfully for user with ID {userId}.", user.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
