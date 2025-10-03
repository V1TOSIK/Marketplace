namespace AuthModule.Application.Auth.Commands.Register
{
    public class RegisterRequest
    {
        public string Credential { get; set; } = string.Empty; // Email or PhoneNumber
        public string Password { get; set; } = string.Empty;
    }
}
