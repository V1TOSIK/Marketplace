using MediatR;

namespace AuthModule.Application.Auth.Commands.LoguotFromDevice
{
    public class LogoutFromDeviceCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;

        public LogoutFromDeviceCommand(Guid userId, string refreshToken)
        {
            UserId = userId;
            RefreshToken = refreshToken;
        }
    }
}
