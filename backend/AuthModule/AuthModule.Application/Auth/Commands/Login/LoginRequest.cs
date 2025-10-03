namespace AuthModule.Application.Auth.Commands.Login
{
    public class LoginRequest
    {
        public string Credential { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public LoginRequest(string credential, string password)
        {
            Credential = credential;
            Password = password;
        }
    }
}
