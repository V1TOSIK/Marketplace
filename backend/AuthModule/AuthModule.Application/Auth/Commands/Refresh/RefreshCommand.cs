using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.Auth.Commands.Refresh
{
    public class RefreshCommand : IRequest<AuthResult>
    {
        public Guid DeviceId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;

        public RefreshCommand(Guid deviceId, string refreshToken)
        {
            DeviceId = deviceId;
            RefreshToken = refreshToken;
        }
    }
}
