using AuthModule.Application.Auth.Commands.AddEmail;
using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using AuthModule.Domain.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AuthModule.Application.Auth.Commands.AddPhone
{
    public class AddPhoneCommandHandler : IRequestHandler<AddPhoneCommand>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<AddPhoneCommandHandler> _logger;
        public AddPhoneCommandHandler(IAuthUserRepository authUserRepository,
            ICurrentUserService currentUserService,
            IAuthUnitOfWork unitOfWork,
            ILogger<AddPhoneCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(AddPhoneCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                _logger.LogWarning("[Auth Module] No authenticated user found for adding phone.");
                throw new UnauthorizedAccessException("No authenticated user found.");
            }
            var user = await _authUserRepository.GetByIdAsync(userId.Value, false, false, cancellationToken);
            if (await _authUserRepository.IsEmailRegisteredAsync(command.Phone, cancellationToken))
            {
                _logger.LogWarning("[Auth Module] Phone {Phone} is already registered.", command.Phone);
                throw new EmailAlreadyExistsException($"Phone {command.Phone} is already registered.");
            }
            user.AddPhone(command.Phone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Auth Module] Phone {Phone} added for user with ID {UserId}.", command.Phone, userId);
        }
    }
}
