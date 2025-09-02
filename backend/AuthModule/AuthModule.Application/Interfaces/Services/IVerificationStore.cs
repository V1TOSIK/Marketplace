namespace AuthModule.Application.Interfaces.Services
{
    public interface IVerificationStore
    {
        Task SaveCodeAsync(string key, string code, CancellationToken cancellationToken);
        Task<bool> VerifyCodeAsync(string key, string code, CancellationToken cancellationToken);
    }
}
