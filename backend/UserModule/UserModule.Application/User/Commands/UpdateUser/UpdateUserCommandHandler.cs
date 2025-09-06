using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.SharedKernel.Interfaces;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;
using UserModule.Domain.Entities;

namespace UserModule.Application.User.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        public UpdateUserCommandHandler(IUserRepository userRepository,
            IUserUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                _logger.LogWarning("[User Module] User ID is empty. Cannot update profile.");
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var user = await _userRepository.GetByIdWithPhoneNumbersAsync(userId.Value, cancellationToken, false, false);

            user.UpdateUser(request.Name, request.Location, request.PhoneNumbers);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[User Module] Profile for user {Name} with ID {userId} has been updated.", user.Name, userId);
        }
    }
}
