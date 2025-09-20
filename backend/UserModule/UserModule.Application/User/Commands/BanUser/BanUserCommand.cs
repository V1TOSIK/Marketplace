using MediatR;

namespace UserModule.Application.User.Commands.BanUser
{
    public class BanUserCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string? Reason { get; set; } = "Violation of terms of service";
        public BanUserCommand(Guid userId, string? reason)
        {
            UserId = userId;
            Reason = reason;
        }

        public BanUserCommand(Guid userId, BanUserRequest request)
        {
            UserId = userId;
            Reason = request.BanReason;
        }
    }
}
