using Microsoft.EntityFrameworkCore;
using SharedKernel.Interfaces;

namespace SharedKernel.UnitOfWork
{
    public class MediaUnitOfWork<TContext> : IMediaUnitOfWork where TContext : DbContext
    {
        private readonly TContext _dbContext;

        public MediaUnitOfWork(TContext dbContext)
        {
            _dbContext = dbContext;
            Console.WriteLine($"[UserUnitOfWork] DbContext HashCode: {_dbContext.GetHashCode()}");
        }
        public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            if (_dbContext.Database.CurrentTransaction != null)
            {
                await action();
                return;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back: {ex.Message}");
                throw;
            }
        }
    }
}
