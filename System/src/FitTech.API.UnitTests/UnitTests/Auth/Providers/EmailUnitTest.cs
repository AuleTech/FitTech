using FitTech.Application.Auth.Configuration;
using FitTech.Application.Auth.Services;
using FitTech.Domain.Entities;
using FitTech.Domain.Interfaces;
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
            
            await service.SendEmailAsync("user@test.com", "Welcome", "<b>Hello</b>", "WelcomeEmail");
       
            await resend.Received(1).EmailSendAsync(Arg.Is<EmailMessage>(msg =>
                msg.To.Contains("user@test.com") &&
                msg.Subject == "Welcome" &&
                msg.HtmlBody == "<b>Hello</b>"));
        
    }
    
    [Test]
    public async Task Should_Log_Email_When_Sent()
    {
        // Arrange
        var resend = Substitute.For<IResend>();
        var logger = Substitute.For<ILogger<EmailService>>();
        var repo = Substitute.For<IEmailRepository>();
        var expectedId = Guid.NewGuid();
       
        
        resend.EmailSendAsync(Arg.Any<EmailMessage>())
            .Returns(Task.FromResult(new ResendResponse<Guid>(Guid.Empty, new ResendRateLimit())), Task.CompletedTask);
        
        resend.EmailRetrieveAsync(Arg.Any<Guid>())
            .Returns(_ => Task.FromResult(new ResendResponse<EmailReceipt>(new EmailReceipt(), new ResendRateLimit())));

        var service = new EmailService(resend, logger, repo, new SecretsSettings());
        
        // Act
        await service.SendEmailAsync("log@test.com", "Subject Test", "<p>Message Content</p>", "LogEmailType");

        // Assert
        await repo.Received(1).AddAsync(Arg.Is<Email>(e =>
            e.ToEmail == "log@test.com" &&
            e.Message == "<p>Message Content</p>" &&
            e.TypeMessage == "LogEmailType"
        ));
    }
    [Test]
    public async Task Shouldnt_Send_Email_Successful_When_Null()
    {
        var resend = Substitute.For<IResend>();
        var logger = Substitute.For<ILogger<EmailService>>();
        var repo = Substitute.For<IEmailRepository>();

        resend.EmailSendAsync(Arg.Any<EmailMessage>())
            .Returns(Task.FromResult(new ResendResponse<Guid>(Guid.Empty, new ResendRateLimit())), Task.CompletedTask);
        
        resend.EmailRetrieveAsync(Arg.Any<Guid>())
            .Returns(_ => Task.FromResult(new ResendResponse<EmailReceipt>(new EmailReceipt(), new ResendRateLimit())));

        var service = new EmailService(resend, logger, repo, new SecretsSettings());
            
        await service.SendEmailAsync("user@test.com", "Welcome", "<b>Hello</b>", "WelcomeEmail");
       
        await resend.DidNotReceive().EmailSendAsync(Arg.Is<EmailMessage>(msg =>
            msg.To.Contains("user@test.com") &&
            msg.Subject == "Welcome" &&
            msg.HtmlBody == String.Empty));
        
        await resend.DidNotReceive().EmailSendAsync(Arg.Is<EmailMessage>(msg =>
            msg.To.Contains("user@test.com") &&
            msg.Subject == String.Empty &&
            msg.HtmlBody == String.Empty));
        
        await resend.DidNotReceive().EmailSendAsync(Arg.Is<EmailMessage>(msg =>
            msg.To.Contains(String.Empty) &&
            msg.Subject == String.Empty &&
            msg.HtmlBody == String.Empty));
    }
    
    [Test]
    public async Task Email_dont_Create_Log_Null()
    {
        // Arrange
        var resend = Substitute.For<IResend>();
        var logger = Substitute.For<ILogger<EmailService>>();
        var repo = Substitute.For<IEmailRepository>();
        var expectedId = Guid.NewGuid();
       
        
        resend.EmailSendAsync(Arg.Any<EmailMessage>())
            .Returns(Task.FromResult(new ResendResponse<Guid>(Guid.Empty, new ResendRateLimit())), Task.CompletedTask);

        resend.EmailRetrieveAsync(Arg.Any<Guid>())
            .Returns(_ => Task.FromResult(new ResendResponse<EmailReceipt>(new EmailReceipt(), new ResendRateLimit())));

        var service = new EmailService(resend, logger, repo,  new SecretsSettings());
        
        // Act
        await service.SendEmailAsync("log@test.com", "Subject Test", "<p>Message Content</p>", "LogEmailType");

        // Assert 
        await repo.DidNotReceive().AddAsync(Arg.Is<Email>(e =>
            e.ToEmail == "log@test.com" &&
            e.Message == "<p>Message Content</p>" &&
            e.TypeMessage == String.Empty
        ));
        
        
        await repo.DidNotReceive().AddAsync(Arg.Is<Email>(e =>
            e.ToEmail == "log@test.com" &&
            e.Message == String.Empty &&
            e.TypeMessage == String.Empty
        ));
        
        await repo.DidNotReceive().AddAsync(Arg.Is<Email>(e =>
            e.ToEmail == String.Empty &&
            e.Message == String.Empty &&
            e.TypeMessage == String.Empty
        ));
        
    }
   
}
