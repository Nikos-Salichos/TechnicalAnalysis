namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IRabbitMqService
    {
        public void PublishMessage<T>(T message);
        public Task<IEnumerable<T>> ConsumeMessageAsync<T>();
    }
}
