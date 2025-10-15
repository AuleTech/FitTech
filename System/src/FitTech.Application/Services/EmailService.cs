using AuleTech.Core.Resiliency;
using FitTech.Application.Configuration;
using FitTech.Domain.Aggregates.EmailAggregate;
using FitTech.Domain.Repositories;
using FitTech.Domain.Templates;
using Microsoft.Extensions.Logging;
using Resend;

namespace FitTech.Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, IEmailTemplate template, CancellationToken cancellationToken);
}

//TODO: Resend has a very constrained rate limit. We need to reimplement this service.
internal sealed class EmailService : IEmailService
{
    private readonly IEmailRepository _emailRepository;
    private readonly ILogger<EmailService> _logger;
    private readonly IResend _resend;
    private readonly ResendSettings _settings;

    public EmailService(IResend resend, ILogger<EmailService> logger, IEmailRepository emailRepository,
        ResendSettings settings)
    {
        _resend = resend;
        _logger = logger;
        _emailRepository = emailRepository;
        _settings = settings;
    }

    public async Task SendEmailAsync(string to, IEmailTemplate template, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to);

        var message = new EmailMessage
        {
            From = _settings.EmailFitTech!, To = { to }, Subject = template.Subject, HtmlBody = template.GetBody()
        };
        
        var response = await ResilientOperations.Default.RetryIfNeededAsync(
            async _ => await _resend.EmailSendAsync(message, cancellationToken), 5, TimeSpan.FromMilliseconds(500),
            cancellationToken: cancellationToken);

        await CreateLogEmailResetAsync();

        async Task CreateLogEmailResetAsync()
        {
            var emailLog = new Email(response.Content, to, template.MessageType,
                response.Success ? nameof(EmailStatus.Delivered) : "Failed");
            await _emailRepository.AddAsync(emailLog, cancellationToken);
        }
    }
}
