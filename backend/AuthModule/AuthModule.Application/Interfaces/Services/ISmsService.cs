namespace AuthModule.Application.Interfaces.Services
{
    public interface ISmsService
    {
        Task SendAsync(string to, string message, CancellationToken cancellationToken);
    }
}
