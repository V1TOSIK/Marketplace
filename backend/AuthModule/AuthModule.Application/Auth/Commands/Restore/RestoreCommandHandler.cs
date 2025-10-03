using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Models;
using Microsoft.Extensions.Logging;
using MediatR;
using AuthModule.Application.Exceptions;

namespace AuthModule.Application.Auth.Commands.Restore
{
    public class RestoreCommandHandler : IRequestHandler<RestoreCommand, AuthResult>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IAuthService _authService;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<RestoreCommandHandler> _logger;
        public RestoreCommandHandler(
            IAuthUserRepository authUserRepository,
            IAuthService authService,
            IAuthUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ILogger<RestoreCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _authService = authService;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<AuthResult> Handle(RestoreCommand command, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(command.UserId, cancellationToken);

            if (!_passwordHasher.VerifyHashedPassword(user.Password ?? "", command.Request.Password))
            {
                _logger.LogWarning("[Auth Module(RestoreCommandHandler)] Invalid security stamp provided for user with ID {UserId}.", command.UserId);
                throw new InvalidPasswordException("Invalid Password.");
            }

            AuthResult response = null!;
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                user.Restore();
                var failResponse = _authService.CheckIfInvalid(user);
                if (failResponse != null)
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    response = failResponse;
                }
                else
                {
                    response = await _authService.BuildAuthResult(user, command.DeviceId, cancellationToken: cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }, cancellationToken);
            _logger.LogInformation("[Auth Module(RestoreCommandHandler)] User with ID {UserId} restored successfully.", command.UserId);
            return response;
        }
    }
}
