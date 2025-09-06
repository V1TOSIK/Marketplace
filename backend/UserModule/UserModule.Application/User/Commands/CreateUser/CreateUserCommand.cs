using MediatR;

namespace UserModule.Application.User.Commands.CreateUser
{
    public class CreateUserCommand : IRequest
    {
        public CreateUserCommand(string name, string location, IEnumerable<string> phoneNumbers)
        {
            Name = name;
            Location = location;
            foreach (var phoneNumber in phoneNumbers)
                PhoneNumbers.Add(phoneNumber);
        }

        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<string> PhoneNumbers { get; set; } = [];
    }
}
