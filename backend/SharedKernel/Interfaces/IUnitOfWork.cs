using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Interfaces
{
    public interface IUnitOfWork<TContext> where TContext : DbContext
    {
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken);
    }
}
