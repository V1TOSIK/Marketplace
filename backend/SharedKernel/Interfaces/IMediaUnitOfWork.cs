namespace SharedKernel.Interfaces
{
    public interface IMediaUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken);
    }
}
