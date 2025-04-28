using Resend;

namespace FitTech.Application.Auth.Services;

public class EmailService : IEmailService
{
    private readonly IResend _resend;
    
    public EmailService( IResend resend)
    {
        _resend = resend;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {

        var message = new EmailMessage
        {
            From = "delivered@resend.dev",
            To =  {  to },
            Subject = subject,
            HtmlBody = htmlBody,
        };
        await _resend.EmailSendAsync( message );
    }
}

