using MediatR;

namespace AuthModule.Application.Auth.Commands.SetPassword
{
    public class SetPasswordCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string Password { get; set; } = string.Empty;

        public SetPasswordCommand(Guid userId, string password)
        {
            UserId = userId;
            Password = password;
        }

        public SetPasswordCommand(Guid userId, SetPasswordRequest request)
        {
            UserId = userId;
            Password = request.NewPassword;
        }
    }
}
