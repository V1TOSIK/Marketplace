using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;
using AuthModule.Domain.Exceptions;

namespace AuthModule.Application.Auth.Commands.SetPhone
{
    public class SetPhoneCommandHandler : IRequestHandler<SetPhoneCommand>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<SetPhoneCommandHandler> _logger;
        public SetPhoneCommandHandler(IAuthUserRepository authUserRepository,
            IAuthUnitOfWork unitOfWork,
            ILogger<SetPhoneCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(SetPhoneCommand command, CancellationToken cancellationToken)
        {
            var user = await _authUserRepository.GetByIdAsync(command.UserId, cancellationToken);

            if (await _authUserRepository.IsPhoneNumberRegisteredAsync(command.Request.Phone, cancellationToken))
            {
                _logger.LogWarning("[Auth Module(AddPhoneCommandHandler)] Phone {Phone} is already registered.", command.Request.Phone);
                throw new PhoneNumberAlreadyExistsException($"Phone {command.Request.Phone} is already registered.");
            }

            user.SetPhone(command.Request.Phone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Auth Module(AddPhoneCommandHandler)] Phone {Phone} added for user with ID {UserId}.", command.Request.Phone, command.UserId);
        }
    }
}
