using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.Auth.Commands.Restore
{
    public class RestoreCommand : IRequest<AuthResult>
    {
        public Guid UserId { get; set; }
        public RestoreRequest Request { get; set; }

        public RestoreCommand(Guid userId, RestoreRequest request)
        {
            UserId = userId;
            Request = request;
        }
    }
}
