using MediatR;

namespace AuthModule.Application.Auth.Commands.AddEmail
{
    public class AddEmailCommand : IRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
