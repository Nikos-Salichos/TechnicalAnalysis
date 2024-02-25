namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICommunication
    {
        public Task CreateAttachmentSendMessage<T>(IEnumerable<T> data, string fileName);
    }
}
