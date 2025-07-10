using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace AuthModule.Application.Dtos.Requests
{
    public class ChangePasswordRequest
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
