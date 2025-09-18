using MediatR;

namespace AuthModule.Application.Auth.Commands.AddPhone
{
    public class AddPhoneCommand : IRequest
    {
        public string Phone { get; set; } = string.Empty;
    }
}
