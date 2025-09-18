using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.Auth.Commands.Restore
{
    public class RestoreCommand : IRequest<AuthResult>
    {
        public RestoreCommand(Guid userId)
        {
            UserId = userId;
        }
        public Guid UserId { get; set; }
    }
}
