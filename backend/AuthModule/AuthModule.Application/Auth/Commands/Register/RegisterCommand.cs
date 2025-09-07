using AuthModule.Application.Models;
using MediatR;

namespace AuthModule.Application.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<AuthResult>
    {
        public string Credential { get; set; } = string.Empty; // Email or PhoneNumber
        public string Password { get; set; } = string.Empty;
    }
}
