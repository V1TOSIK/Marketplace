namespace AuthModule.Application.Interfaces.Services
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(Guid userId, string role);

    }
}
