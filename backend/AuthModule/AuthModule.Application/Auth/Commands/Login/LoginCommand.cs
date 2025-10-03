using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.Auth.Commands.Login
{
    public class LoginCommand : IRequest<AuthResult>
    {
        public Guid DeviceId { get; set; }
        public LoginRequest Request { get; set; } = null!;

        public LoginCommand(Guid deviceId, LoginRequest request)
        {
            DeviceId = deviceId;
            Request = request;
        }
    }
}
