using MimeKit;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICommunication
    {
        public Task CreateAttachmentSendMessage<T>(IEnumerable<T> data);
        public Task SendMessage(List<MimePart> message);
        public void CreateAttachment<T>(string fileName, string filetype, List<MimePart> attachments, IEnumerable<T> data);
    }
}
