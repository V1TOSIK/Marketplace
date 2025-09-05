using UserModule.Application.Dtos;

namespace UserModule.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserModule.Domain.Entities.User> GetByIdAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false, bool includeBanned = false);
        Task<UserDto> GetByIdWithPhoneNumbersAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false, bool includeBanned = false);
        Task<IEnumerable<string>> GetUserPhoneNumbersAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(UserModule.Domain.Entities.User user, CancellationToken cancellationToken);
        Task HardDeleteAsync(Guid userId, CancellationToken cancellationToken);
    }
}
