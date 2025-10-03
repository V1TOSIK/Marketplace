using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Models;
using AuthModule.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthModule.Application.OAuth.Commands.OAuthLogin
{
    public class OAuthLoginCommandHandler : IRequestHandler<OAuthLoginCommand, AuthResult>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IAuthService _authService;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<OAuthLoginCommandHandler> _logger;
        public OAuthLoginCommandHandler(IAuthUserRepository authUserRepository,
            IAuthService authService,
            IAuthUnitOfWork unitOfWork,
            ILogger<OAuthLoginCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _authService = authService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AuthResult> Handle(OAuthLoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByOAuthAsync(command.Provider, command.ProviderUserId, cancellationToken);
            if (user == null)
            {
                var email = string.IsNullOrWhiteSpace(command.Email) ? null : command.Email;
                user = AuthUser.Create(email, null, null);
                user.AddExternalLogin(command.ProviderUserId, command.Provider);
                await _authUserRepository.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var failResponse = _authService.CheckIfInvalid(user);
            if (failResponse != null)
                return failResponse;

            AuthResult response = await _authService.BuildAuthResult(user, command.DeviceId, cancellationToken: cancellationToken);

            _logger.LogInformation("[Auth Module(OAuthLoginCommandHandler)] User with Id: {UserId} logged in by OAuth successfully.", user.Id);
            return response;

        }
    }
}
