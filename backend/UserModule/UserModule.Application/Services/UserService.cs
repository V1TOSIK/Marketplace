using UserModule.Application.Dtos.Requests;
using Microsoft.Extensions.Logging;
using UserModule.Domain.Entities;
using UserModule.Application.Interfaces.Services;
using UserModule.Application.Interfaces.Repositories;
using UserModule.Application.Interfaces;
using UserModule.Application.Dtos;

namespace UserModule.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepository,
            IUserUnitOfWork unitOfWork,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CreateNewProfile(Guid userId, CreateUserRequest request, CancellationToken cancellationToken)
        {
            var user = UserModule.Domain.Entities.User.Create(userId, request.Name, request.Location);
            foreach (var phone in request.PhoneNumbers)
            {
                user.AddPhoneNumber(phone);
            }
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"New profile created for user {user.Name} with ID {userId}.");
        }

        public async Task UpdateProfile(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithPhoneNumbersAsync(userId, cancellationToken, false, false);

            user.(request.Name, request.Location, request.PhoneNumbers);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Profile for user {user.Name} with ID {userId} has been updated.");
        }
    }
}
