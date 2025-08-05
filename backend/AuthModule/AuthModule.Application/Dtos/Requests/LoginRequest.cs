namespace AuthModule.Application.Dtos.Requests
{
    public class LoginRequest
    {
        public string? ProviderUserId { get; set; } = string.Empty;
        public string? Provider { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
