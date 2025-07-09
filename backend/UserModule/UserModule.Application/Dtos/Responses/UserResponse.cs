using UserModule.Domain.Entities;

namespace UserModule.Application.Dtos.Responses
{
    public class UserResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public IEnumerable<string> PhoneNumbers { get; set; } = [];
    }
}
