using AuthModule.Application.Dtos.Requests;

namespace AuthModule.Application.Interfaces.Services
{
    public interface IVerificationService
    {
        public Task SendVerificationCode(string destination, CancellationToken cancellationToken);
        public Task<bool> VerifyCode(VerifyCodeRequest request, CancellationToken cancellationToken);
    }
}
