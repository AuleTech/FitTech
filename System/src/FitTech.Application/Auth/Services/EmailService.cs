using FitTech.Application.Auth.Configuration;
using FitTech.Domain.Entities;
using FitTech.Domain.Interfaces;
using FitTech.Domain.Templates;
using Microsoft.Extensions.Logging;
using Resend;

namespace FitTech.Application.Auth.Services;

internal sealed class EmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailRepository _emailRepository;
    private readonly SecretsSettings _settings;

    public EmailService(IResend resend, ILogger<EmailService> logger, IEmailRepository emailRepository,
        SecretsSettings settings)
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
        var response = await _resend.EmailSendAsync(message, cancellationToken);

        await CreateLogEmailResetAsync();
        
        async Task CreateLogEmailResetAsync()
        {
            var emailLog = new Email(response.Content, to, template.MessageType, response.Success ? nameof(EmailStatus.Delivered) : "Failed");
            await _emailRepository.AddAsync(emailLog, cancellationToken);
        }
    }
}
