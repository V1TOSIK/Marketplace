using AuthModule.Application.Dtos.Responses;
using AuthModule.Domain.Entities;

namespace AuthModule.Application.Models
{
    public class AuthResult
    {
        public AuthorizeResponse Response { get; set; } = new ();
        public RefreshToken RefreshToken { get; set; } = null!;
    }
}
