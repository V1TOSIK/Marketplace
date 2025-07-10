namespace SharedKernel.Interfaces
{
    public interface IUserUnitOfWork
    {
        Task SaveChangesAsync();
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
