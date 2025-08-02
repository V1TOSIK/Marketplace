using UserModule.Application.Dtos.Requests;
using UserModule.Application.Dtos.Responses;
using UserModule.Application.Interfaces;
using UserModule.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using UserModule.Domain.Entities;
using UserModule.Domain.Exceptions;
using SharedKernel.Interfaces;
using UserModule.Application.Exceptions;

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
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken, false);
            var response = new UserResponse
            {
                Name = user.Name,
                Location = user.Location,
                PhoneNumbers = user.GetPhoneNumbersValue()
            };
            _logger.LogInformation($"Profile retrieved for user {user.Name} with ID {userId}.");
            return response;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            var userResponses = users.Select(user => new UserResponse
            {
                Name = user.Name,
                Location = user.Location,
                PhoneNumbers = user.GetPhoneNumbersValue()
            }).ToList();
            _logger.LogInformation($"Retrieved {userResponses.Count} users from the repository.");
            return userResponses;
        }

        public async Task CreateNewProfile(Guid userId, CreateUserRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError("CreateUserRequest cannot be null.");
                throw new BadRequestException("CreateUserRequest cannot be null.");
            }
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
            if (request == null)
            {
                _logger.LogError("UpdateUserRequest cannot be null.");
                throw new BadRequestException("UpdateUserRequest cannot be null.");
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken, false);

            if (!string.IsNullOrWhiteSpace(request.Name))
                user.UpdateName(request.Name);
            if (!string.IsNullOrWhiteSpace(request.Location))
                user.UpdateLocation(request.Location);
            if (request.PhoneNumbers != null && request.PhoneNumbers.Any())
            {
                foreach (var phone in request.PhoneNumbers)
                {
                    if (!string.IsNullOrWhiteSpace(phone) && !user.GetPhoneNumbers().Any(p => p.PhoneNumber.Value == phone))
                    {
                        user.AddPhoneNumber(phone);
                    }
                }
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Profile for user {user.Name} with ID {userId} has been updated.");
        }

        public async Task SoftDeleteProfile(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken, false);
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

        public async Task AddPhoneNumber(Guid userId, string phone, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken, false);
            user.AddPhoneNumber(phone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Phone number {phone} has been added to user {user.Name} with ID {userId}.");
        }

        public async Task RemovePhoneNumber(Guid userId, int phoneId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken, false);
            user.RemovePhoneNumber(phoneId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Phone number with ID {phoneId} has been removed from user {user.Name} with ID {userId}.");
        }
    }
}
