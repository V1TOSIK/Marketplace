using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using AuthModule.Domain.Exceptions;
using MediatR;

namespace AuthModule.Application.Auth.Commands.SetEmail
{
    public class SetEmailCommandHandler : IRequestHandler<SetEmailCommand>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<SetEmailCommandHandler> _logger;
        public SetEmailCommandHandler(IAuthUserRepository authUserRepository,
            IAuthUnitOfWork unitOfWork,
            ILogger<SetEmailCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(SetEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(command.UserId, cancellationToken);

            if (await _authUserRepository.IsEmailRegisteredAsync(command.Request.Email, cancellationToken))
            {
                _logger.LogWarning("[Auth Module(AddEmailCommandHandler)] Email {Email} is already registered.", command.Request.Email);
                throw new EmailAlreadyExistsException($"Email {command.Request.Email} is already registered.");
            }
            user.SetEmail(command.Request.Email);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Auth Module(AddEmailCommandHandler)] Email {Email} added for user with ID {UserId}.", command.Request.Email, command.UserId);
        }
    }
}
