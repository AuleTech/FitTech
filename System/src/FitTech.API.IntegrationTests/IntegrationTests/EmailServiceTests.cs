using FitTech.Application;
using FitTech.Application.Auth.Configuration;
using FitTech.Application.Auth.Services;
using FitTech.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;

namespace FitTech.Api.Tests.IntegrationTests;

public class EmailServiceTest
{
    private static IEmailService? _sut; //TODO: Use the SUT
    public static IResend? _resend;
    
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
        _resend = sp.GetRequiredService<IResend>();
    }
    
    //TODO: Refactor
    [Test]
    [Timeout(30_000)]
    public async Task CanSendAndRetrieveEmailAsync(CancellationToken cancellationToken)
    {
        
        var message = new EmailMessage
        {
            From ="admin@fittech.es",
            To = "delivered@resend.dev",
            Subject = "Test",
            HtmlBody = "htmlBody",
        };

        var response = await _resend!.EmailSendAsync(message, cancellationToken);
        
        await Task.Delay(3000, cancellationToken);
        var delivered = await _resend.EmailRetrieveAsync(response.Content, cancellationToken);
        var status = delivered.Content.LastEvent.ToString();

       await Assert.That(status).IsNotNull();
       await Assert.That(status).IsEquivalentTo("Delivered"); 
    }
}
