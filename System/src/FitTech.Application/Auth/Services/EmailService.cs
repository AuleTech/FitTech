using FitTech.Application.Auth.Configuration;
using FitTech.Domain.Entities;
using FitTech.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Resend;
using FitTech.Persistence.Repositories;
using Npgsql.Replication.PgOutput.Messages;

namespace FitTech.Application.Auth.Services;

public sealed class EmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailRepository _emailRepository;
    private readonly DbSecretsSettings _dbSettings;
    public EmailService( IResend resend, ILogger<EmailService> logger, IEmailRepository emailRepository, DbSecretsSettings dbSettings)
    {
        _resend = resend;
        _logger = logger;
        _emailRepository = emailRepository;
        _dbSettings = dbSettings;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody, string typeMessage)
    {

        var message = new EmailMessage
        {
            From =  _dbSettings.EmailFitTech!,
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

        await CreateLogEmailResetAsync(response.Content, to, htmlBody, typeMessage);
    }
    
    private async Task CreateLogEmailResetAsync(Guid emailId, String ToEmail, String Message, String TypeMessage)
    {
        var emailLog = new Email(emailId, ToEmail, Message, TypeMessage);
        await _emailRepository.AddAsync(emailLog);
    }
}

