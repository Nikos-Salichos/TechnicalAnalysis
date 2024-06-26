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
        private bool isFirstTime = true;
        private const string _queue = "taQueue";
        private const string _deadLetterQueue = "taQueue.dlq";
        private const string _exchangeName = "taExchange";
        private const string _deadLetterExchange = "taExchange.dlq";
        private const string _routingKey = "taKey";
        private const string _deadLetterRoutingKey = "taKey.dlq";

        private readonly ConnectionFactory connectionFactory = new()
        {
            HostName = rabbitMqSetting.CurrentValue.Hostname,
            Port = rabbitMqSetting.CurrentValue.Port,
            UserName = rabbitMqSetting.CurrentValue.Username,
            Password = rabbitMqSetting.CurrentValue.Password,
        };

        public void PublishMessage<T>(T message)
        {
            var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            if (isFirstTime)
            {
                channel.ExchangeDeclare(_deadLetterExchange, "direct", true, false);
                channel.QueueDeclare(_deadLetterQueue, durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(_deadLetterQueue, _deadLetterExchange, _deadLetterRoutingKey);

                // Configure primary queue with dead-letter exchange
                var arguments = new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", _deadLetterExchange },
                        { "x-dead-letter-routing-key", _deadLetterRoutingKey }
                    };

                channel.ExchangeDeclare(_exchangeName, "direct", true, false);
                channel.QueueDeclare(_queue, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
                channel.QueueBind(_queue, _exchangeName, _routingKey);

                isFirstTime = false;
            }

            var json = JsonSerializer.Serialize(message, JsonHelper.JsonSerializerOptions);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: _exchangeName, routingKey: _routingKey, basicProperties: properties, body: body);
        }

        public async Task<List<T>> ConsumeMessageAsync<T>()
        {
            using var connection = connectionFactory.CreateConnection();
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

            channel.BasicConsume(queue: _queue, autoAck: true, consumer: consumer);

            // Wait for some time or an event indicating that enough messages have been received.
            await Task.Delay(TimeSpan.FromSeconds(5));

            return receivedMessages;
        }
    }
}