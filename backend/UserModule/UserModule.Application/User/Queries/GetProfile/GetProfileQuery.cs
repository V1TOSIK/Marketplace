using MediatR;
using UserModule.Application.Dtos;

namespace UserModule.Application.User.Queries.GetProfile
{
    public class GetProfileQuery : IRequest<UserDto>
    {
        public Guid UserId { get; set; }
    }
}
