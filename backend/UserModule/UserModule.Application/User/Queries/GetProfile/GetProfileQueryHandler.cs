using MediatR;
using Microsoft.Extensions.Logging;
using UserModule.Application.Dtos;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.User.Queries.GetProfile
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<GetProfileQueryHandler> _logger;

        public GetProfileQueryHandler(IUserRepository userRepository,
            IUserUnitOfWork UnitOfWork,
            ILogger<GetProfileQueryHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = UnitOfWork;
            _logger = logger;
        }

        public async Task<UserDto> Handle(GetProfileQuery command, CancellationToken cancellationToken)
        {
            var response = await _userRepository.GetByIdWithPhoneNumbersAsync(command.UserId, cancellationToken, false, false);
            
            _logger.LogInformation("[User Module] Profile retrieved for user {Name} with ID {UserId}.", response.Name, command.UserId);
            return response;
        }
    }
}
