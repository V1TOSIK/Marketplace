namespace SharedKernel.Interfaces
{
    public interface IAuthUnitOfWork
    {
        Task SaveChangesAsync();
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
