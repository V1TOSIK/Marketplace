using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace AuthModule.Application.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IVerificationStore _store;
        private readonly ILogger<VerificationService> _logger;
        public VerificationService(IEmailService emailService,
            ISmsService smsService,
            IVerificationStore store,
            ILogger<VerificationService> logger)
        {
            _emailService = emailService;
            _smsService = smsService;
            _store = store;
            _logger = logger;
        }

        public async Task SendVerificationCode(string destination, CancellationToken cancellationToken)
        {
            var code = GenerateVerificationCodeAsync();
            await _store.SaveCodeAsync(destination, code, cancellationToken);

            if (destination.Contains("@"))
                await _emailService.SendAsync(destination, "Verification Code", $"Your verification code is: {code}", cancellationToken);
            else
                await _smsService.SendAsync(destination, $"Your verification code is: {code}", cancellationToken);
        }

        public async Task<bool> VerifyCode(VerificationRequest request, CancellationToken cancellationToken)
        {
            if(await _store.VerifyCodeAsync(request.Destination, request.Code, cancellationToken))
            {
                _logger.LogInformation("Verification successful for {Destination}", request.Destination);
                return true;
            }
            return false;
        }

        private string GenerateVerificationCodeAsync()
        {
            var code = new Random().Next(100000, 999999).ToString();
            return code;
        }
    }
}
