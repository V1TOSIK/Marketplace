using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using MediatR;
using AuthModule.Domain.Exceptions;
using AuthModule.Domain.ValueObjects;
using SharedKernel.Interfaces;

namespace AuthModule.Application.Auth.Commands.AddEmail
{
    public class AddEmailCommandHandler : IRequestHandler<AddEmailCommand>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<AddEmailCommandHandler> _logger;
        public AddEmailCommandHandler(IAuthUserRepository authUserRepository,
            ICurrentUserService currentUserService,
            IAuthUnitOfWork unitOfWork,
            ILogger<AddEmailCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(AddEmailCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                _logger.LogWarning("[Auth Module] No authenticated user found for adding email.");
                throw new UnauthorizedAccessException("No authenticated user found.");
            }
            var user = await _authUserRepository.GetByIdAsync(userId.Value, cancellationToken);
            if (await _authUserRepository.IsEmailRegisteredAsync(command.Email, cancellationToken))
            {
                _logger.LogWarning("[Auth Module] Email {Email} is already registered.", command.Email);
                throw new EmailAlreadyExistsException($"Email {command.Email} is already registered.");
            }
            user.AddEmail(command.Email);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Auth Module] Email {Email} added for user with ID {UserId}.", command.Email, userId);
        }
    }
}
