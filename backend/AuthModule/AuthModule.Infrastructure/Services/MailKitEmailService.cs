using AuthModule.Application.Interfaces.Services;
using AuthModule.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AuthModule.Infrastructure.Services
{
    public class MailKitEmailService : IEmailService
    {
        private readonly EmailOptions _options;
        public MailKitEmailService(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }
        public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.SenderName, _options.Username));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
