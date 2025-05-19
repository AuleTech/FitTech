namespace FitTech.Application.Auth.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlbody, string typeMessage);
}
