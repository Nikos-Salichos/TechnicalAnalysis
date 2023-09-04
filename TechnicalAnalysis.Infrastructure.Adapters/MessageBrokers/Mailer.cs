using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Messages;

namespace TechnicalAnalysis.Infrastructure.Adapters.MessageBrokers
{
    public class Mailer : IMailer
    {
        private readonly IOptionsMonitor<MailSettings> _mailSettings;

        public Mailer(IOptionsMonitor<MailSettings> settings)
        {
            _mailSettings = settings;
        }

        public async Task SendAsync(MailData mailData, CancellationToken ct = default)
        {
            try
            {
                // Initialize a new instance of the MimeKit.MimeMessage class
                var mail = new MimeMessage();

                #region Sender / Receiver
                // Sender
                mail.From.Add(new MailboxAddress(_mailSettings.CurrentValue.DisplayName, mailData.From ?? _mailSettings.CurrentValue.From));
                mail.Sender = new MailboxAddress(mailData.DisplayName ?? _mailSettings.CurrentValue.DisplayName, mailData.From ?? _mailSettings.CurrentValue.From);

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
                #endregion

                #region Content

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

                #endregion

                #region Send Mail

                using var smtp = new SmtpClient();

                if (_mailSettings.CurrentValue.UseSSL)
                {
                    await smtp.ConnectAsync(_mailSettings.CurrentValue.Host, _mailSettings.CurrentValue.Port, SecureSocketOptions.SslOnConnect, ct);
                }
                else if (_mailSettings.CurrentValue.UseStartTls)
                {
                    await smtp.ConnectAsync(_mailSettings.CurrentValue.Host, _mailSettings.CurrentValue.Port, SecureSocketOptions.StartTls, ct);
                }
                await smtp.AuthenticateAsync(_mailSettings.CurrentValue.EmailAddress, _mailSettings.CurrentValue.Password, ct);
                await smtp.SendAsync(mail, ct);
                await smtp.DisconnectAsync(true, ct);

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
