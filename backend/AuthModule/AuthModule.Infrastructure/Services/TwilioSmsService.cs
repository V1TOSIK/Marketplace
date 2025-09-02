using AuthModule.Application.Interfaces.Services;
using AuthModule.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AuthModule.Infrastructure.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly SmsOptions _options;
        public TwilioSmsService(IOptions<SmsOptions> options)
        {
            _options = options.Value;
            TwilioClient.Init(_options.AccountSid, _options.AuthToken);
        }
        public async Task SendAsync(string to, string message, CancellationToken cancellationToken)
        {
            await MessageResource.CreateAsync(
            to: new PhoneNumber(to),
            from: new PhoneNumber(_options.FromPhone),
            body: message
        );
        }
    }
}
