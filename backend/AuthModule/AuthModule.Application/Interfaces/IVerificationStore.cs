namespace AuthModule.Application.Interfaces
{
    public interface IVerificationStore
    {
        Task SaveCodeAsync(string key, string code, CancellationToken cancellationToken);
        Task<bool> VerifyCodeAsync(string key, string code, CancellationToken cancellationToken);
    }
}
