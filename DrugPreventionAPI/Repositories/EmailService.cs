using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DrugPreventionAPI.Repositories
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        public EmailService(IOptions<EmailSettings> opts)
            => _settings = opts.Value;

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_settings.FromName, _settings.From));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;

            var body = new BodyBuilder { HtmlBody = htmlBody };
            msg.Body = body.ToMessageBody();

            using var client = new SmtpClient();
            // kết nối đến Gmail SMTP
            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(
                _settings.UserName,
                _settings.Password);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
        }
    }
}
