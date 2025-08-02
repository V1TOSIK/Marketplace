namespace SharedKernel.Interfaces
{
    public interface IUserRestorer
    {
        Task RestoreUserAsync(Guid userId, CancellationToken cancellationToken);
    }  
}
