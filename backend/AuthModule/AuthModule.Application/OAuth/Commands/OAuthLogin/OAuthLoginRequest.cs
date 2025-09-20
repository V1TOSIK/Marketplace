namespace AuthModule.Application.OAuth.Commands.OAuthLogin
{
    public class OAuthLoginRequest
    {
        public string? ProviderUserId { get; set; } = string.Empty;
        public string? Provider { get; set; }
    }
}
