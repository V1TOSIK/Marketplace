using MediatR;
using Microsoft.Extensions.Logging;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.User.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        public UpdateUserCommandHandler(IUserRepository userRepository,
            IUserUnitOfWork unitOfWork,
            ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithPhoneNumbersAsync(command.UserId, cancellationToken);

            user.UpdateUser(command.Name, command.Location, command.PhoneNumbers);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[User Module(UpdateUserCommandHandler)] Profile for user {Name} with ID {userId} has been updated.", user.Name, user.Id);
        }
    }
}
