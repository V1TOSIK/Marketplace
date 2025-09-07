namespace AuthModule.Application.Dtos.Requests
{
    public class OAuthLoginRequest
    {
        public string? ProviderUserId { get; set; } = string.Empty;
        public string? Provider { get; set; }
    }
}
