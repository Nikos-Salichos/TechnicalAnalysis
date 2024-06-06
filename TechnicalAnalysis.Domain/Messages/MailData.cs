using MimeKit;

namespace TechnicalAnalysis.Domain.Messages
{
    public class MailData(List<string> to,
        string subject,
        string? body = null,
        List<MimePart>? attachments = null,
        string? from = null,
        string? displayName = null,
        string? replyTo = null,
        string? replyToName = null,
        List<string>? bcc = null,
        List<string>? cc = null)
    {
        // Receiver
        public List<string> To { get; } = to;
        public List<string> Bcc { get; } = bcc ?? [];

        public List<string> Cc { get; } = cc ?? [];

        // Sender
        public string? From { get; } = from;

        public string? DisplayName { get; } = displayName;

        public string? ReplyTo { get; } = replyTo;

        public string? ReplyToName { get; } = replyToName;

        // Content
        public string Subject { get; } = subject;

        public string? Body { get; } = body;

        public List<MimePart>? Attachments { get; } = attachments;
    }
}
