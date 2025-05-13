using FitTech.Application;
using FitTech.Application.Auth.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;

namespace FitTech.Api.Tests.IntegrationTests;

//TODO: Look in Resend how to properly test
public class EmailServiceTest
{
    private static IEmailService? _sut;
    public static IResend? _resend;
    
    [Before(Class)]
    public static void Setup()
    {
        var configurationBuilder =
            new ConfigurationBuilder().AddInMemoryCollection(new KeyValuePair<string, string?>[]
            {
                new KeyValuePair<string, string?>("Resend:ApiToken","re_emUGHF3R_GSSKHxQxbYvx4jGKuv3tVsmA")
            });

        var configuration = configurationBuilder.Build();
        
        var serviceCollection = new ServiceCollection().AddHttpClient().AddLogging();
        serviceCollection.AddEmailService(configuration);

        var sp = serviceCollection.BuildServiceProvider();

        _sut = sp.GetRequiredService<IEmailService>();
    }

    [Test]
    [Timeout(30_000)]
    public async Task CanSendEmailAsync(CancellationToken cancellationToken)
    {
        await _sut!.SendEmailAsync("", "Test", "<strong>it works!</strong>", "ResetEmail");
    }
    
    [Test]
    [Timeout(30_000)]
    public async Task CanSendAndRetrieveEmailAsync(CancellationToken cancellationToken)
    {
        
         var message = _sut!.SendEmailAsync(
            to: "example@email.com", 
            subject: "Test", 
            htmlbody: "<strong>it works!</strong>",
            "ResetEmail"
        );
         
        var retrieved = await _resend!.EmailRetrieveAsync(Guid.Parse(message.Id.ToString()));

       await Assert.That(retrieved.Content.LastEvent).IsNotNull();
       await Assert.That(retrieved.Content.LastEvent).IsEquivalentTo("Delivered"); 
    }
}
