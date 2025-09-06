using MediatR;
using Microsoft.Extensions.Logging;
using UserModule.Application.Dtos;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.User.Queries.GetProfile
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetProfileQueryHandler> _logger;

        public GetProfileQueryHandler(IUserRepository userRepository,
            ILogger<GetProfileQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserDto> Handle(GetProfileQuery command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithPhoneNumbersAsync(command.UserId, cancellationToken, false, false);
            
            var response = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Location = user.Location,
                PhoneNumbers = user.PhoneNumbers.Select(pn => pn.PhoneNumber.Value).ToList()
            };

            _logger.LogInformation("[User Module] Profile retrieved for user {Name} with ID {UserId}.", response.Name, command.UserId);
            return response;
        }
    }
}
