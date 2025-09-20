using MediatR;
using Microsoft.Extensions.Logging;
using UserModule.Application.Dtos;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.User.Queries.GetMyProfile
{
    public class GetMyProfilyQueryHandler : IRequestHandler<GetMyProfileQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetMyProfilyQueryHandler> _logger;

        public GetMyProfilyQueryHandler(IUserRepository userRepository,
            ILogger<GetMyProfilyQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserDto> Handle(GetMyProfileQuery command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithPhoneNumbersAsync(command.UserId, cancellationToken);

            var response = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Location = user.Location,
                PhoneNumbers = user.PhoneNumbers.Select(pn => pn.PhoneNumber.Value).ToList()
            };

            _logger.LogInformation("[User Module(GetMyProfileQueryHandler)] Profile retrieved for user {Name} with ID {UserId}.", response.Name, command.UserId);
            return response;
        }
    }
}
