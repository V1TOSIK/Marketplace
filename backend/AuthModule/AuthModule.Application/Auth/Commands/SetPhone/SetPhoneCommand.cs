using MediatR;

namespace AuthModule.Application.Auth.Commands.SetPhone
{
    public class SetPhoneCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string Phone { get; set; } = string.Empty;

        public SetPhoneCommand(Guid userId, string phone)
        {
            UserId = userId;
            Phone = phone;
        }

        public SetPhoneCommand(Guid userId, SetPhoneRequest request)
        {
            UserId = userId;
            Phone = request.Phone;
        }
    }
}
