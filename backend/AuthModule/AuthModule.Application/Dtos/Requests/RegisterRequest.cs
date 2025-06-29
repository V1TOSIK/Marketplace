using AuthModule.Domain.Enums;

namespace AuthModule.Application.Dtos.Requests
{
    public class RegisterRequest
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "guest";
    }
}
