namespace UserModule.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<Domain.Entities.User> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Domain. Entities.User> GetByIdWithPhoneNumbersAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetUserPhoneNumbersAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Entities.User user, CancellationToken cancellationToken = default);
        Task HardDeleteAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
