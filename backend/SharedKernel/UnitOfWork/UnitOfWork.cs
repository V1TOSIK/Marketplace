using Microsoft.EntityFrameworkCore;
using SharedKernel.Interfaces;

public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _dbContext;

    public UnitOfWork(TContext dbContext) => _dbContext = dbContext;

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
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}
