using MediatR;

namespace AuthModule.Application.Auth.Commands.LogoutFromAllDevices
{
    public class LogoutFromAllDevicesCommand : IRequest
    {
        public Guid UserId { get; set; }
        public LogoutFromAllDevicesCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}
