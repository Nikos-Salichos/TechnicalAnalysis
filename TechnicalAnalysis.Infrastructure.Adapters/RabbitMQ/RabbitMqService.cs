using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Settings;

namespace TechnicalAnalysis.Infrastructure.Adapters.RabbitMQ
{
    public class RabbitMqService(IOptionsMonitor<RabbitMqSetting> rabbitMqSetting) : IRabbitMqService
    {
        private bool isFirstTime = true;
        private const string _queueName = "taQueue";
        private const string _exchangeName = "taExchange";
        private const string _routingKey = "taKey";
        private ConnectionFactory connectionFactory = new()
        {
            HostName = rabbitMqSetting.CurrentValue.Hostname,
            Port = rabbitMqSetting.CurrentValue.Port,
            UserName = rabbitMqSetting.CurrentValue.Username,
            Password = rabbitMqSetting.CurrentValue.Password,
        };

        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        public void PublishMessage<T>(T message)
        {
            var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            if (isFirstTime)
            {
                //channel.ExchangeDelete(_exchangeName);
                channel.ExchangeDeclare(_exchangeName, "direct", true, false);

                //channel.QueueDelete(_queueName);
                channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);

                channel.QueueBind(_queueName, _exchangeName, _routingKey);

                isFirstTime = false;
            }

            var json = JsonSerializer.Serialize(message, jsonSerializerOptions);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: _exchangeName, routingKey: _routingKey, basicProperties: properties, body: body);
        }

        public async Task<IEnumerable<T>> ConsumeMessageAsync<T>()
        {
            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            var receivedMessages = new List<T>();

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deserializedMessage = JsonSerializer.Deserialize<T>(message, jsonSerializerOptions);

                if (deserializedMessage != null)
                {
                    receivedMessages.Add(deserializedMessage);
                }
            };

            channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            // Wait for some time or an event indicating that enough messages have been received.
            // You can customize this part based on your application logic.
            await Task.Delay(TimeSpan.FromSeconds(10));

            return receivedMessages;
        }
    }
}