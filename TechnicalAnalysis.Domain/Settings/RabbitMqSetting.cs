﻿namespace TechnicalAnalysis.Domain.Settings
{
    public class RabbitMqSetting
    {
        public string Hostname { get; init; } = string.Empty;
        public int Port { get; init; }
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
