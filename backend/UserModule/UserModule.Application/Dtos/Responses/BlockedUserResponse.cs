namespace UserModule.Application.Dtos.Responses
{
    public class BlockedUserResponse
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<string> PhoneNumbers { get; set; } = [];
    }
}
