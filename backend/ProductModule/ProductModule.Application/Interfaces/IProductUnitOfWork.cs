namespace ProductModule.Application.Interfaces
{
    public interface IProductUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken);
    }
}
