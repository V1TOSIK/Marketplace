﻿namespace SharedKernel.Interfaces
{
    public interface IUnitOfWork 
    {
        Task SaveChangesAsync();
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
