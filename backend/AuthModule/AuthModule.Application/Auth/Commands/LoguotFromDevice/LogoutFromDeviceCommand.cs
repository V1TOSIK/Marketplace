using MediatR;

namespace AuthModule.Application.Auth.Commands.LoguotFromDevice
{
    public class LogoutFromDeviceCommand : IRequest
    {
        public LogoutFromDeviceCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
