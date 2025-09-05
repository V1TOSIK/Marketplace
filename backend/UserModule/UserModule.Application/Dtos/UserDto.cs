using UserModule.Domain.Entities;

namespace UserModule.Application.Dtos
{
    public class UserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public IEnumerable<string> PhoneNumbers { get; set; } = [];
    }
}
