using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Models;
using Microsoft.Extensions.Logging;
using AuthModule.Application.Exceptions;
using AuthModule.Domain.Exceptions;
using MediatR;

namespace AuthModule.Application.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthService _authService;
        private readonly ILogger<LoginCommandHandler> _logger;
        public LoginCommandHandler(
            IAuthUserRepository authUserRepository,
            IPasswordHasher passwordHasher,
            IAuthService authService,
            ILogger<LoginCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _passwordHasher = passwordHasher;
            _authService = authService;
            _logger = logger;
        }

        public async Task<AuthResult> Handle(LoginCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _authUserRepository.GetByCredentialAsync(command.Credential, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("[Auth Module(LoginCommandHandler)] User with this credential: {Credential} was not found.", command.Credential);
                throw new UserNotFoundException("User with this credential was not found.");
            }
            if (user.Password == null)
            {
                _logger.LogWarning("[Auth Module(LoginCommandHandler)] User with this credential: {Credential} has no password set.", command.Credential);
                throw new MissingPasswordException("User has no password set.");
            }
            if (!_passwordHasher.VerifyHashedPassword(user.Password, command.Password))
            {
                _logger.LogWarning("[Auth Module(LoginCommandHandler)] Invalid password for user with credential: {Credential}.", command.Credential);
                throw new InvalidPasswordException("Invalid password.");
            }

            var failResponse = _authService.CheckIfInvalid(user);
            if (failResponse != null)
                return failResponse;

            AuthResult response = await _authService.BuildAuthResult(user, cancellationToken: cancellationToken);

            _logger.LogInformation("[Auth Module(LoginCommandHandler)] User with Id: {UserId} logged in successfully.", user.Id);
            return response;
        }
    }
}
