using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.OAuth.Commands.OAuthLogin
{
    public class OAuthLoginCommand : IRequest<AuthResult>
    {
        public string Provider { get; set; }
        public string ProviderUserId { get; set; }
        public string? Email { get; set; }
        public Guid DeviceId { get; set; }

        public OAuthLoginCommand(string provider,
            string providerUserId,
            string? email,
            Guid deviceId)
        {
            Provider = provider;
            ProviderUserId = providerUserId;
            Email = email;
            DeviceId = deviceId;
        }
    }
}
