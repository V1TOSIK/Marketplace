namespace AuthModule.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(Guid userId, string role);

    }
}
