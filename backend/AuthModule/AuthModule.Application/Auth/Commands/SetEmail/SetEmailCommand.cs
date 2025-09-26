using MediatR;

namespace AuthModule.Application.Auth.Commands.SetEmail
{
    public class SetEmailCommand : IRequest
    {
        public Guid UserId { get; set; }
        public SetEmailRequest Request { get; set; }

        public SetEmailCommand(Guid userId, SetEmailRequest request)
        {
            UserId = userId;
            Request = request;
        }
    }
}
