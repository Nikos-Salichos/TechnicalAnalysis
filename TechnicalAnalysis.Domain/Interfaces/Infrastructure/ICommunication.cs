namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICommunication
    {
        public Task CreateAttachmentSendMessage<T>(List<T> data, string fileName);
    }
}
