﻿using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Settings;

namespace TechnicalAnalysis.Infrastructure.Host.RabbitMQ
{
    public class RabbitMqService(IOptionsMonitor<RabbitMqSetting> rabbitMqSetting) : IRabbitMqService
    {
        private bool _isFirstTime = true;
        private const string Queue = "taQueue";
        private const string DeadLetterQueue = "taQueue.dlq";
        private const string ExchangeName = "taExchange";
        private const string DeadLetterExchange = "taExchange.dlq";
        private const string RoutingKey = "taKey";
        private const string DeadLetterRoutingKey = "taKey.dlq";

        private readonly ConnectionFactory _connectionFactory = new()
        {
            HostName = rabbitMqSetting.CurrentValue.Hostname,
            Port = rabbitMqSetting.CurrentValue.Port,
            UserName = rabbitMqSetting.CurrentValue.Username,
            Password = rabbitMqSetting.CurrentValue.Password,
        };

        public void PublishMessage<T>(T message)
        {
            var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            if (_isFirstTime)
            {
                channel.ExchangeDeclare(DeadLetterExchange, "direct", true, false);
                channel.QueueDeclare(DeadLetterQueue, durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(DeadLetterQueue, DeadLetterExchange, DeadLetterRoutingKey);

                // Configure primary queue with dead-letter exchange
                var arguments = new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", DeadLetterExchange },
                        { "x-dead-letter-routing-key", DeadLetterRoutingKey }
                    };

                channel.ExchangeDeclare(ExchangeName, "direct", true, false);
                channel.QueueDeclare(Queue, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
                channel.QueueBind(Queue, ExchangeName, RoutingKey);

                _isFirstTime = false;
            }

            var json = JsonSerializer.Serialize(message, JsonHelper.JsonSerializerOptions);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: ExchangeName, routingKey: RoutingKey, basicProperties: properties, body: body);
        }

        public async Task<List<T>> ConsumeMessageAsync<T>()
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            var receivedMessages = new List<T>();

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var deserializedMessage = JsonSerializer.Deserialize<T>(message, JsonHelper.JsonSerializerOptions);
                if (deserializedMessage != null)
                {
                    receivedMessages.Add(deserializedMessage);
                }
            };

            channel.BasicConsume(queue: Queue, autoAck: true, consumer: consumer);

            // Wait for some time or an event indicating that enough messages have been received.
            await Task.Delay(TimeSpan.FromSeconds(5));

            return receivedMessages;
        }
    }
}