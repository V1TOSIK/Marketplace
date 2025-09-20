using MediatR;

namespace UserModule.Application.User.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public List<string>? PhoneNumbers { get; set; }

        public UpdateUserCommand(Guid id, string? name, string? location, List<string>? phoneNumbers)
        {
            UserId = id;
            Name = name;
            Location = location;
            PhoneNumbers = phoneNumbers;
        }

        public UpdateUserCommand(Guid id, UpdateUserRequest request)
        {
            UserId = id;
            Name = request.Name;
            Location = request.Location;
            PhoneNumbers = request.PhoneNumbers;
        }
    }
}
