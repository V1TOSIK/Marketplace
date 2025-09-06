using MediatR;

namespace UserModule.Application.User.Commands.UnbanUser
{
    public class UnbanUserCommand : IRequest
    {
        public Guid UserId { get; set; }
        public UnbanUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}
