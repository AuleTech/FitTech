using FitTech.Application.Auth.Configuration;
using FitTech.Application.Auth.Services;
using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;
using FitTech.Domain.Templates.EmailsTemplates;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Resend;

namespace FitTech.API.UnitTests.UnitTests.Auth.Providers;

public class EmailServiceTests
{
    [Test]
    public async Task Should_Send_Email_Successful()
    {
        
            var resend = Substitute.For<IResend>();
            var logger = Substitute.For<ILogger<EmailService>>();
            var repo = Substitute.For<IEmailRepository>();

            resend.EmailSendAsync(Arg.Any<EmailMessage>())
                  .Returns(Task.FromResult(new ResendResponse<Guid>(Guid.Empty, new ResendRateLimit())), Task.CompletedTask);
            
            resend.EmailRetrieveAsync(Arg.Any<Guid>())
                .Returns(_ => Task.FromResult(new ResendResponse<EmailReceipt>(new EmailReceipt(), new ResendRateLimit())));

            var service = new EmailService(resend, logger, repo, new SecretsSettings());
            
            await service.SendEmailAsync("user@test.com", ResetPasswordTemplate.Create("test"), CancellationToken.None);
       
            await resend.Received(1).EmailSendAsync(Arg.Any<EmailMessage>());
            await repo.Received(1).AddAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>());
    }
    
    [Test]
    public async Task SendEmailAsync_WhenToIsNull_ShouldThrowArgumentNullException()
    {
        var resend = Substitute.For<IResend>();
        var logger = Substitute.For<ILogger<EmailService>>();
        var repo = Substitute.For<IEmailRepository>();

        resend.EmailSendAsync(Arg.Any<EmailMessage>())
            .Returns(Task.FromResult(new ResendResponse<Guid>(Guid.Empty, new ResendRateLimit())), Task.CompletedTask);
        
        resend.EmailRetrieveAsync(Arg.Any<Guid>())
            .Returns(_ => Task.FromResult(new ResendResponse<EmailReceipt>(new EmailReceipt(), new ResendRateLimit())));

        var service = new EmailService(resend, logger, repo, new SecretsSettings());

        await Assert.ThrowsAsync<ArgumentException>(async () => await service.SendEmailAsync(string.Empty,
            ResetPasswordTemplate.Create("test"),
            CancellationToken.None));

    }
}
