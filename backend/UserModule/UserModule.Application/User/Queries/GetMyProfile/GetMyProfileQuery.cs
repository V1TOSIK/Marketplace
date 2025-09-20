using MediatR;
using UserModule.Application.Dtos;

namespace UserModule.Application.User.Queries.GetMyProfile
{
    public class GetMyProfileQuery : IRequest<UserDto>
    {
        public GetMyProfileQuery(Guid userId)
        {
            UserId = userId;
        }
        public Guid UserId { get; set; }
    }
}
