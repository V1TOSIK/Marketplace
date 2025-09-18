using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.OAuth.Commands
{
    public class OAuthLoginCommand : IRequest<AuthResult>
    {
        public string Provider { get; set; }
        public string ProviderUserId { get; set; }
        public string? Email { get; set; }
        public OAuthLoginCommand(string provider, string providerUserId, string? email)
        {
            Provider = provider;
            ProviderUserId = providerUserId;
            Email = email;
        }
    }
}
