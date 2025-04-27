using Resend;

namespace FitTech.Application.Auth.Services;

public class EmailService
{
    private readonly IResend _resend;
    
    public EmailService( IResend resend)
    {
        _resend = resend;
    }

    public async Task SendEmailAsync()
    {
        
        var message = new EmailMessage();
        message.From = "you@example.com";
        message.To.Add( "user@gmail.com" );
        message.Subject = "hello world";
        message.HtmlBody = "<strong>it works!</strong>";

        await _resend.EmailSendAsync( message );
    }
}

