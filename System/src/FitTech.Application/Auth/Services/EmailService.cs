using FitTech.Application.Auth.Configuration;
using FitTech.Domain.Entities;
using FitTech.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Resend;


namespace FitTech.Application.Auth.Services;

public sealed class EmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailRepository _emailRepository;
    private readonly SecretsSettings _dbSettings;
    public EmailService( IResend resend, ILogger<EmailService> logger, IEmailRepository emailRepository, SecretsSettings dbSettings)
    {
        _resend = resend;
        _logger = logger;
        _emailRepository = emailRepository;
        _dbSettings = dbSettings;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody, string typeMessage)
    {

        try
        {

            var message = new EmailMessage
            {
                From = _dbSettings.EmailFitTech!, To = { to }, Subject = subject, HtmlBody = htmlBody,
            };
            var response = await _resend.EmailSendAsync(message);

            var delivered = await _resend.EmailRetrieveAsync(response.Content);

            var status = delivered.Content.LastEvent.ToString();
            
            await CreateLogEmailResetAsync(response.Content, to, htmlBody, typeMessage, status!);
            
        }
        catch (ResendException e)
        {
            _logger.LogError("Resend error: {ErrorType}", e.ErrorType);
        }
        
    }
    
    private async Task CreateLogEmailResetAsync(Guid emailId, String ToEmail, String Message, String TypeMessage, String emailStatus)
    {
        var emailLog = new Email(emailId, ToEmail, Message, TypeMessage, emailStatus);
        await _emailRepository.AddAsync(emailLog);
    }
}

