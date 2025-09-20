using MediatR;
using UserModule.Application.Dtos;

namespace UserModule.Application.User.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<UserDto>
    {
        public GetUserProfileQuery(Guid userId)
        {
            UserId = userId;
        }
        public Guid UserId { get; set; }
    }
}
