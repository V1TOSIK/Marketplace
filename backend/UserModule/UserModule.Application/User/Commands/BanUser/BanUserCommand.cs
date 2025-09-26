using MediatR;

namespace UserModule.Application.User.Commands.BanUser
{
    public class BanUserCommand : IRequest
    {
        public Guid UserId { get; set; }
        public BanUserRequest Request { get; set; }

        public BanUserCommand(Guid userId, BanUserRequest request)
        {
            UserId = userId;
            Request = request;
        }
    }
}
