using TechnicalAnalysis.Domain.Messages;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IMailer
    {
        Task SendAsync(MailData mailData, CancellationToken ct);
    }
}
