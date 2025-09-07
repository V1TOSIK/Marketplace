namespace AuthModule.Application.Dtos.Responses
{
    public class AuthorizeResponse
    {
        public Guid? UserId { get; set; }
        public string? Role { get; set; }
        public string? AccessToken { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBanned { get; set; }
        public bool CanRestore { get; set; }
        public string? Message { get; set; }
    }
}
