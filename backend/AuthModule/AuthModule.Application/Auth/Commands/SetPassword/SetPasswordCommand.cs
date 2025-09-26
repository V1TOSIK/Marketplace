using MediatR;

namespace AuthModule.Application.Auth.Commands.SetPassword
{
    public class SetPasswordCommand : IRequest
    {
        public Guid UserId { get; set; }
        public SetPasswordRequest Request { get; set; }

        public SetPasswordCommand(Guid userId, SetPasswordRequest request)
        {
            UserId = userId;
            Request = request;
        }
    }
}
