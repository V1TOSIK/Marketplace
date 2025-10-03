using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<AuthResult>
    {
        public Guid DeviceId { get; set; }
        public RegisterRequest Request { get; set; } = null!;

        public RegisterCommand(Guid deviceId, RegisterRequest request)
        {
            DeviceId = deviceId;
            Request = request;
        }
    }
}
