namespace SharedKernel.Interfaces
{
    public interface IProductUnitOfWork
    {
        Task SaveChangesAsync();
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
