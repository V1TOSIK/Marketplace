using MediatR;

namespace UserModule.Application.User.Commands.BanUser
{
    public class BanUserCommand : IRequest
    {
        public Guid UserId { get; set; }
        public BanUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}
