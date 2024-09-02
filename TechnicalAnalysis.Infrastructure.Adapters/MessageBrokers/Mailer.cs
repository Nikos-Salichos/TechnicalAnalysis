using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Messages;

namespace TechnicalAnalysis.Infrastructure.Adapters.MessageBrokers
{
    public class Mailer(IOptionsMonitor<MailSettings> settings) : IMailer
    {
        private readonly MailSettings _settings = settings?.CurrentValue ?? throw new ArgumentNullException(nameof(settings));

        public async Task SendAsync(MailData mailData, CancellationToken cancellationToken)
        {
            var mail = CreateEmailMessage(mailData);

            using var smtp = new SmtpClient();
            await ConnectToSmtpServerAsync(smtp, cancellationToken);
            await smtp.AuthenticateAsync(_settings.EmailAddress, _settings.Password, cancellationToken);
            await smtp.SendAsync(mail, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }

        private MimeMessage CreateEmailMessage(MailData mailData)
        {
            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));
            mail.Sender = new MailboxAddress(mailData.DisplayName ?? _settings.DisplayName, mailData.From ?? _settings.From);

            AddMailAddresses(mail.To, mailData.To);
            if (mailData.ReplyTo != null)
            {
                AddMailAddresses(mail.ReplyTo, [mailData.ReplyTo], mailData.ReplyToName);
            }

            AddMailAddresses(mail.Bcc, mailData.Bcc);
            AddMailAddresses(mail.Cc, mailData.Cc);

            var bodyBuilder = new BodyBuilder
            {
                TextBody = mailData.Body
            };

            if (mailData.Attachments != null)
            {
                foreach (var attachment in mailData.Attachments)
                {
                    bodyBuilder.Attachments.Add(attachment);
                }
            }

            mail.Subject = mailData.Subject;
            mail.Body = bodyBuilder.ToMessageBody();

            return mail;
        }

        private static void AddMailAddresses(InternetAddressList addressList, List<string>? addresses, string? displayName = null)
        {
            if (addresses == null)
            {
                return;
            }

            foreach (var address in addresses.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                if (MailboxAddress.TryParse(address.Trim(), out var mailboxAddress))
                {
                    addressList.Add(string.IsNullOrEmpty(displayName) 
                        ? mailboxAddress 
                        : new MailboxAddress(displayName, mailboxAddress.Address));
                }
            }
        }

        private async Task ConnectToSmtpServerAsync(SmtpClient smtp, CancellationToken cancellationToken)
        {
            SecureSocketOptions options;
            if (_settings.UseSSL)
            {
                options = SecureSocketOptions.SslOnConnect;
            }
            else if (_settings.UseStartTls)
            {
                options = SecureSocketOptions.StartTls;
            }
            else
            {
                options = SecureSocketOptions.Auto;
            }

            await smtp.ConnectAsync(_settings.Host, _settings.Port, options, cancellationToken);
        }
    }
}
