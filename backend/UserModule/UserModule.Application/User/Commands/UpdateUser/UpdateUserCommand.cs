using MediatR;

namespace UserModule.Application.User.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public List<string>? PhoneNumbers { get; set; } = [];
    }
}
