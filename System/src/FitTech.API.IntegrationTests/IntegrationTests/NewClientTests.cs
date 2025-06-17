using FitTech.Application;
using FitTech.Application.Auth.Services;
using FitTech.Application.Dtos.Client;
using FitTech.Application.Services;
using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;
using FitTech.Domain.Templates.EmailsTemplates;
using FitTech.Persistence;
using FitTech.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Resend;

namespace FitTech.Api.Tests.IntegrationTests;

public class NewClientTests
{
    private static IClientService? _sut;
    private static IServiceProvider? _serviceProvider; 
    
    public static void Setup()
    {
        var serviceCollection = new ServiceCollection().AddLogging();
        serviceCollection.AddTransient<IClientService, ClientService>();
        serviceCollection.AddTransient<IClientRepository, ClientRepository>();
        serviceCollection.AddInMemorydb(Guid.NewGuid().ToString());
      
        var sp = serviceCollection.BuildServiceProvider();
        _sut = sp.GetRequiredService<IClientService>();
        _serviceProvider = sp;
       
    }
    
    [Test]
    [Timeout(30_000)]
    public async Task AddNewClient(CancellationToken cancellationToken)
    {
        await _sut!.AddAsync(new AddClientDto(), cancellationToken);
        
        await using var dbContext = _serviceProvider?.GetRequiredService<FitTechDbContext>();
         
         var result = await dbContext!.ClientTable.FindAsync(cancellationToken);
         
        await Assert.That(result!.Name).IsEqualTo("Yeray");
        
      
    }
}
