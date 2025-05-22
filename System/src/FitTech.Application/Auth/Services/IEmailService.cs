using FitTech.Domain.Templates;

namespace FitTech.Application.Auth.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to,IEmailTemplate template, CancellationToken cancellationToken);
}
