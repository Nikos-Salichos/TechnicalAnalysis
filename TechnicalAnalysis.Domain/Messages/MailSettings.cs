using System.ComponentModel.DataAnnotations;

namespace TechnicalAnalysis.Domain.Messages
{
    public class MailSettings
    {
        public string? DisplayName { get; init; }

        [Required]
        public string? From { get; init; }

        [Required]
        public string? EmailAddress { get; init; }

        [Required]
        public string? Password { get; init; }

        [Required]
        public string? Host { get; init; }

        [Required]
        public int Port { get; init; }

        [Required]
        public bool UseSSL { get; init; }

        [Required]
        public bool UseStartTls { get; init; }
    }
}
