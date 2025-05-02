using FitTech.Domain.Entities;
using FitTech.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Resend;
using FitTech.Persistence.Repositories;
namespace FitTech.Application.Auth.Services;

internal sealed class EmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailRepository _emailRepository;
    public EmailService( IResend resend, ILogger<EmailService> logger, IEmailRepository emailRepository)
    {
        _resend = resend;
        _logger = logger;
        _emailRepository = emailRepository;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {

        var message = new EmailMessage
        {
            From = "admin@fittech.es",
            To =  {  to },
            Subject = subject,
            HtmlBody = htmlBody,
        };
        var response = await _resend.EmailSendAsync( message );

        //TODO: Do we really need to throw?
        if (!response.Success)
        {
            _logger.LogError("Couldn't send email: {ExceptionMessage}",response.Exception!.Message);
        }

        await CreateLogEmailResetAsync(response.Content, to, htmlBody);
    }
    
    private async Task CreateLogEmailResetAsync(Guid emailId, String ToEmail, String Message)
    {
        var emailLog = new Email(emailId, ToEmail, Message);
        await _emailRepository.AddAsync(emailLog);
    }
}

