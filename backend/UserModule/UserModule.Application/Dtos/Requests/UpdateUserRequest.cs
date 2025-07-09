namespace UserModule.Application.Dtos.Requests
{
    public class UpdateUserRequest
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public List<string?> PhoneNumbers { get; set; } = [];
    }
}
