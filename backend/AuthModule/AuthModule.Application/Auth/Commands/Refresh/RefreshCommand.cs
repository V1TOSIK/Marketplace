using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.Auth.Commands.Refresh
{
    public class RefreshCommand : IRequest<AuthResult>
    {
        public RefreshCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
