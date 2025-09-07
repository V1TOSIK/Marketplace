using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.Auth.Commands.Login
{
    public class LoginCommand : IRequest<AuthResult>
    {
        public string Credential { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
