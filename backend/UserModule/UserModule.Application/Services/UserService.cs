using UserModule.Application.Dtos.Requests;
using UserModule.Application.Dtos.Responses;
using Microsoft.Extensions.Logging;
using UserModule.Domain.Entities;
using SharedKernel.Interfaces;
using UserModule.Application.Interfaces.Services;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserManager _userManager;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepository,
            IUserManager userManager,
            IUserUnitOfWork unitOfWork,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UserResponse> GetProfile(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithPhoneNumbersAsync(userId, cancellationToken, false, false);
            var response = new UserResponse
            {
                Name = user.Name,
                Location = user.Location,
                PhoneNumbers = user.PhoneNumbers
                    .Select(p => p.PhoneNumber.Value)
                    .ToList(),
            };
            _logger.LogInformation($"Profile retrieved for user {user.Name} with ID {userId}.");
            return response;
        }

        public async Task CreateNewProfile(Guid userId, CreateUserRequest request, CancellationToken cancellationToken)
        {
            var user = User.Create(userId, request.Name, request.Location);
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

            user.UpdateUser(request.Name, request.Location, request.PhoneNumbers);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Profile for user {user.Name} with ID {userId} has been updated.");
        }

        public async Task BanProfile(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken, false, false);
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _userManager.Ban(userId, cancellationToken);
                user.Ban();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"Profile for user with ID {userId} has been banned.");
            }, cancellationToken);
        }

        public async Task UnBanProfile(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken, false, true);
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _userManager.UnBan(userId, cancellationToken);
                user.UnBan();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"Profile for user with ID {userId} has been unbanned.");
            }, cancellationToken);
        }

        public async Task SoftDeleteProfile(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken, false, false);
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                user.MarkAsDeleted();
                await _userManager.SoftDeleteUser(userId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            _logger.LogInformation($"Profile for user with ID {userId} has been soft deleted.");
        }

        public async Task HardDeleteProfile(Guid userId, CancellationToken cancellationToken)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _userRepository.HardDeleteAsync(userId, cancellationToken);
                await _userManager.HardDeleteUser(userId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            _logger.LogInformation($"Profile for user with ID {userId} has been hard deleted.");
        }
    }
}
