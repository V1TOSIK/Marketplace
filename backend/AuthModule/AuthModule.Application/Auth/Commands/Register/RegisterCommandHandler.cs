using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using AuthModule.Application.Models;
using AuthModule.Domain.Entities;
using AuthModule.Domain.Exceptions;
using AuthModule.Application.Exceptions;
using MediatR;
using AuthModule.Application.Interfaces;

namespace AuthModule.Application.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthService _authService;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterCommandHandler> _logger;
        public RegisterCommandHandler(
            IAuthUserRepository authUserRepository,
            IPasswordHasher passwordHasher,
            IAuthService authService,
            IAuthUnitOfWork unitOfWork,
            ILogger<RegisterCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _passwordHasher = passwordHasher;
            _authService = authService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AuthResult> Handle(RegisterCommand command, CancellationToken cancellationToken = default)
        {
            var existingUser = await _authUserRepository.GetByCredentialAsync(command.Request.Credential, cancellationToken);

            if (existingUser != null)
            {
                _logger.LogWarning("[Auth Module(RegisterCommandHandler)] User with this credential already exists.");
                throw new UserAlreadyRegisteredException("User with this credential already exists.");
            }

            var hashPassword = _passwordHasher.HashPassword(command.Request.Password);

            AuthResult response = null!;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var user = command.Request.Credential.Contains("@")
                    ? AuthUser.Create(command.Request.Credential, null, hashPassword)
                    : AuthUser.Create(null, command.Request.Credential, hashPassword);
                await _authUserRepository.AddAsync(user, cancellationToken);

                response = await _authService.BuildAuthResult(user, command.DeviceId, cancellationToken: cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

            _logger.LogInformation("[Auth Module(RegisterCommandHandler)] User with id: {UserId} registered successfully.", response.Response.UserId);
            return response;
        }
    }
}
