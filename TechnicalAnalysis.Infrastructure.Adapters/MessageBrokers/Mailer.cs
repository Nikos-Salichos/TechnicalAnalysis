using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Messages;

namespace TechnicalAnalysis.Infrastructure.Adapters.MessageBrokers
{
    public class Mailer(ILogger<Mailer> logger, IOptionsMonitor<MailSettings> settings) : IMailer
    {
        public async Task SendAsync(MailData mailData, CancellationToken cancellationToken)
        {
            var mail = new MimeMessage();

            // Sender
            mail.From.Add(new MailboxAddress(settings.CurrentValue.DisplayName, mailData.From ?? settings.CurrentValue.From));
            mail.Sender = new MailboxAddress(mailData.DisplayName ?? settings.CurrentValue.DisplayName, mailData.From ?? settings.CurrentValue.From);

            // Receiver
            foreach (string mailAddress in mailData.To)
            {
                if (MailboxAddress.TryParse(mailAddress.Trim(), out var value))
                {
                    mail.To.Add(value);
                }
            }

            // Set Reply to if specified in mail data
            if (!string.IsNullOrEmpty(mailData.ReplyTo))
            {
                mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));
            }

            // BCC
            // Check if a BCC was supplied in the request
            if (mailData.Bcc != null)
            {
                // Get only addresses where value is not null or with whitespace. x = value of address
                foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    if (MailboxAddress.TryParse(mailAddress.Trim(), out var value))
                    {
                        mail.Bcc.Add(value);
                    }
                }
            }

            // CC
            // Check if a CC address was supplied in the request
            if (mailData.Cc != null)
            {
                foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    if (MailboxAddress.TryParse(mailAddress.Trim(), out var value))
                    {
                        mail.Cc.Add(value);
                    }
                }
            }

            // Add Content to Mime Message
            var body = new BodyBuilder();
            mail.Subject = mailData.Subject;
            body.TextBody = mailData.Body;

            if (mailData.Attachments != null)
            {
                foreach (var attachment in mailData.Attachments)
                {
                    body.Attachments.Add(attachment);
                }
            }

            mail.Body = body.ToMessageBody();


            using var smtp = new SmtpClient();

            if (settings.CurrentValue.UseSSL)
            {
                await smtp.ConnectAsync(settings.CurrentValue.Host, settings.CurrentValue.Port, SecureSocketOptions.SslOnConnect, cancellationToken);
            }
            else if (settings.CurrentValue.UseStartTls)
            {
                await smtp.ConnectAsync(settings.CurrentValue.Host, settings.CurrentValue.Port, SecureSocketOptions.StartTls, cancellationToken);
            }
            await smtp.AuthenticateAsync(settings.CurrentValue.EmailAddress, settings.CurrentValue.Password, cancellationToken);
            await smtp.SendAsync(mail, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }
    }
}
