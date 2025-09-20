using MediatR;

namespace AuthModule.Application.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

        public ChangePasswordCommand(Guid userId, ChangePasswordRequest request)
        {
            UserId = userId;
            CurrentPassword = request.CurrentPassword;
            NewPassword = request.NewPassword;
        }
    }
}
