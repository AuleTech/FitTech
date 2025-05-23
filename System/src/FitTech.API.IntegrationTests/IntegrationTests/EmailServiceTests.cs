using FitTech.Application;
using FitTech.Application.Auth.Configuration;
using FitTech.Application.Auth.Services;
using FitTech.Domain.Interfaces;
using FitTech.Domain.Templates.EmailsTemplates;
using FitTech.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;

namespace FitTech.Api.Tests.IntegrationTests;

public class EmailServiceTest
{
    private static IEmailService? _sut; //TODO: Use the SUT
    private static IServiceProvider? _serviceProvider; 
        
    [Before(Class)]
    public static void Setup()
    {
        var configurationBuilder =
            new ConfigurationBuilder().AddInMemoryCollection(new KeyValuePair<string, string?>[]
            {
                new KeyValuePair<string, string?>("Resend:ApiToken","re_emUGHF3R_GSSKHxQxbYvx4jGKuv3tVsmA"),
                new KeyValuePair<string, string?>($"SecretsSettings:{nameof(SecretsSettings.EmailFitTech)}", "admin@fittech.es"),
            });

        var configuration = configurationBuilder.Build();
        
        var serviceCollection = new ServiceCollection().AddHttpClient().AddLogging();
        serviceCollection.AddEmailService(configuration).AddInMemorydb(Guid.NewGuid().ToString());

        var sp = serviceCollection.BuildServiceProvider();

        _sut = sp.GetRequiredService<IEmailService>();
        _serviceProvider = sp;
    }
    
    //TODO: Refactor
    [Test]
    [Timeout(30_000)]
    public async Task SendEmailAsync_WhenHappyPath_EmailIsDeliveredAndLogIsSavedInDb(CancellationToken cancellationToken)
    {
        var template = ResetPasswordTemplate.Create("testurl.com");
        await _sut!.SendEmailAsync("delivered@resend.dev", template,
            cancellationToken);

        await using var dbContext = _serviceProvider?.GetRequiredService<FitTechDbContext>();
        var resendApiClient = _serviceProvider?.GetRequiredService<IResend>();
        
        var log = await dbContext!.EmailLog.SingleAsync(cancellationToken);
        await Assert.That(log.EmailStatus).IsEqualTo("Delivered");
        await Assert.That(log.TypeMessage).IsEqualTo(template.MessageType);

        await Task.Delay(3000, cancellationToken); //TODO: Use Polly for Pull
        var emailRetrieved = await resendApiClient!.EmailRetrieveAsync(log.ExternalId, cancellationToken);
        await Assert.That(emailRetrieved.Content.LastEvent.ToString()).IsEqualTo("Delivered");
    }
}
