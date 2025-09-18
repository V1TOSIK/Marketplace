using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthModule.Application.Auth.Commands.Restore
{
    public class RestoreCommandHandler : IRequestHandler<RestoreCommand, AuthResult>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IAuthService _authService;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<RestoreCommandHandler> _logger;
        public RestoreCommandHandler(
            IAuthUserRepository authUserRepository,
            IAuthService authService,
            IAuthUnitOfWork unitOfWork,
            ILogger<RestoreCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _authService = authService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AuthResult> Handle(RestoreCommand command, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(command.UserId, true, true, cancellationToken);

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
                    response = await _authService.BuildAuthResult(user, cancellationToken: cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }, cancellationToken);
            _logger.LogInformation("[Auth Module] User with ID {UserId} restored successfully.", command.UserId);
            return response;
        }
    }
}
