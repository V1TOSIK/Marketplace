namespace UserModule.Application.Dtos.Requests
{
    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<string> PhoneNumbers { get; set; } = [];
        public List<Guid> BlockedUsers { get; set; } = [];
    }
}
