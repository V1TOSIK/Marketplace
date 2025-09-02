using AuthModule.Application.Dtos.Requests;

namespace AuthModule.Application.Interfaces
{
    public interface IVerificationService
    {
        public Task SendVerificationCode(string destination, CancellationToken cancellationToken);
        public Task<bool> VerifyCode(VerificationRequest request, CancellationToken cancellationToken);
    }
}
