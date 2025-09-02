namespace SharedKernel.Interfaces
{
    public interface IUserUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken);
    }
}
