using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.Auth.Commands.Restore
{
    public class RestoreCommand : IRequest<AuthResult>
    {
        public Guid UserId { get; set; }
        public string Password { get; set; } = string.Empty;

        public RestoreCommand(Guid userId, RestoreRequest request)
        {
            UserId = userId;
            Password = request.Password;
        }
    }
}
