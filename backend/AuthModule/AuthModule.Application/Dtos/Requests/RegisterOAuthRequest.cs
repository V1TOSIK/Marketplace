namespace AuthModule.Application.Dtos.Requests
{
    public class RegisterOAuthRequest
    {
        public string ProviderUserId { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
