namespace SharedKernel.Interfaces
{
    public interface IMediaUnitOfWork
    {
        Task SaveChangesAsync();
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
