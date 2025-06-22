using AuthUserModule.Domain.Enums;

namespace AuthUserModule.Application.Dtos.Requests
{
    public class RegisterUserRequest
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "guest";
    }
}
