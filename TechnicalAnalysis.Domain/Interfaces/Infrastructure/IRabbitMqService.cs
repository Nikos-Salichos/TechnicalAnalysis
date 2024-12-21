namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IRabbitMqService
    {
        public Task PublishMessage<T>(T message);
        public Task<List<T>> ConsumeMessageAsync<T>();
    }
}
