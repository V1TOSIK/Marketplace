using MediatR;

namespace UserModule.Application.User.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest
    {
        public Guid UserId { get; set; }
        public UpdateUserRequest Request { get; set; }

        public UpdateUserCommand(Guid id, UpdateUserRequest request)
        {
            UserId = id;
            Request = request;
        }
    }
}
