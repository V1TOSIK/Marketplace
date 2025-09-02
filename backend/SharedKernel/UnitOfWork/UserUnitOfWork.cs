using Microsoft.EntityFrameworkCore;
using SharedKernel.Interfaces;

namespace SharedKernel.UnitOfWork
{
    public class UserUnitOfWork<TContext> : IUserUnitOfWork where TContext : DbContext
    {
        private readonly TContext _dbContext;

        public UserUnitOfWork(TContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _dbContext.SaveChangesAsync(cancellationToken);

        public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken)
        {
            if (_dbContext.Database.CurrentTransaction != null)
            {
                await action();
                return;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await action();
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Console.WriteLine($"Transaction rolled back: {ex.Message}");
                throw;
            }
        }
    }
}
