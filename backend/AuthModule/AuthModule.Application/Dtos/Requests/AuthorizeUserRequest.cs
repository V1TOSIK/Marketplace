namespace AuthModule.Application.Dtos.Requests
{
    public class AuthorizeUserRequest
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
