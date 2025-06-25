namespace AuthModule.Application.Dtos.Responses
{
    public class AuthorizeUserResponse
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = "Guest";

        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
