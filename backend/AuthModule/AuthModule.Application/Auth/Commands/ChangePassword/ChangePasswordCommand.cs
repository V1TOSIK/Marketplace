using MediatR;

namespace AuthModule.Application.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest
    {
        public Guid UserId { get; set; }
        public ChangePasswordRequest Request { get; set; }

        public ChangePasswordCommand(Guid userId, ChangePasswordRequest request)
        {
            UserId = userId;
            Request = request;
        }
    }
}
