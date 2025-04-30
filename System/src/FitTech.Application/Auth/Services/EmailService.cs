using Microsoft.Extensions.Logging;
using Resend;

namespace FitTech.Application.Auth.Services;

internal sealed class EmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly ILogger<EmailService> _logger;
    public EmailService( IResend resend, ILogger<EmailService> logger)
    {
        _resend = resend;
        _logger = logger;
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
    }
}

