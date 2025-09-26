using MediatR;

namespace AuthModule.Application.Auth.Commands.SetPhone
{
    public class SetPhoneCommand : IRequest
    {
        public Guid UserId { get; set; }
        public SetPhoneRequest Request { get; set; }

        public SetPhoneCommand(Guid userId, SetPhoneRequest request)
        {
            UserId = userId;
            Request = request;
        }
    }
}
