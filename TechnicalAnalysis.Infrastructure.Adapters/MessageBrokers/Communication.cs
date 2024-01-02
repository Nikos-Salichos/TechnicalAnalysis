using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Text;
using System.Text.Json;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Messages;

namespace TechnicalAnalysis.Infrastructure.Adapters.MessageBrokers
{
    public class Communication(IMailer mailService, IConfiguration configuration) : ICommunication
    {
        public async Task CreateAttachmentSendMessage<T>(IEnumerable<T> data)
        {
            List<MimePart> mailInformation = [];
            CreateAttachment(nameof(data), ".json", mailInformation, data.ToList());
            await SendMessage(mailInformation);
        }

        public async Task SendMessage(List<MimePart> message)
        {
            var receivers = configuration.GetValue<string>("MailData:To") ?? string.Empty;
            List<string> receiversList = receivers.Split(',').Select(r => r.Trim()).ToList();

            var bccReceivers = configuration.GetValue<string>("MailData:Bcc") ?? string.Empty;
            List<string> bccReceiversList = bccReceivers.Split(',').Select(r => r.Trim()).ToList();

            MailData mailData = new MailData(receiversList, "Coins", null, message, null, null, null, null, bccReceiversList, null);
            await mailService.SendAsync(mailData, new CancellationToken());
        }

        public void CreateAttachment<T>(string fileName, string filetype, List<MimePart> attachments, IEnumerable<T> data)
        {
            string json = JsonSerializer.Serialize(data, JsonHelper.JsonSerializerOptions);

            var byteArray = Encoding.UTF8.GetBytes(json);

            // Create a new attachment with the byte array and specify the content type as "application/json"
            var attachment = new MimePart("application", "json")
            {
                Content = new MimeContent(new MemoryStream(byteArray)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = fileName + filetype
            };

            attachments.Add(attachment);
        }
    }
}
