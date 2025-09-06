using MediatR;

namespace UserModule.Application.User.Commands.DeactivateUser
{
    public class DeactivateUserCommand : IRequest
    {
        public Guid UserId { get; set; }
        public DeactivateUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}
