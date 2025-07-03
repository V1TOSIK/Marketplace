using UserModule.Domain.Entities;

namespace UserModule.Application.Dtos.Responses
{
    public class UserResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public List<string> PhoneNumbers { get; set; } = [];
        public List<User> BlockedUsers { get; set; } = [];
    }
}
