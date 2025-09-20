using MediatR;

namespace AuthModule.Application.Auth.Commands.SetEmail
{
    public class SetEmailCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;

        public SetEmailCommand(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }

        public SetEmailCommand(Guid userId, SetEmailRequest request)
        {
            UserId = userId;
            Email = request.Email;
        }
    }
}
