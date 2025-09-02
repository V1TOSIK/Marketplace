namespace SharedKernel.Interfaces
{
    public interface IUserManager
    {
        Task HardDeleteUser(Guid userId, CancellationToken cancellationToken);
        Task SoftDeleteUser(Guid userId, CancellationToken cancellationToken);
        Task Ban(Guid userId, CancellationToken cancellationToken);
        Task UnBan(Guid userId, CancellationToken cancellationToken);
        Task UpdateRole(Guid userId, string roleText, CancellationToken cancellationToken);
    }
}
