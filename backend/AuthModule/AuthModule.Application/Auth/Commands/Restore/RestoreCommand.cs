using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.Auth.Commands.Restore
{
    public class RestoreCommand : IRequest<AuthResult>
    {
        public Guid UserId { get; set; }
        public Guid DeviceId { get; set; }
        public RestoreRequest Request { get; set; }

        public RestoreCommand(Guid userId,
            Guid deviceId,
            RestoreRequest request)
        {
            UserId = userId;
            DeviceId = deviceId;
            Request = request;
        }
    }
}
