using FitTech.Application.Auth.Configuration;
using FitTech.Application.Auth.Services;
using FitTech.Application.Services;
using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;
using FitTech.Domain.Templates.EmailsTemplates;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Resend;

namespace FitTech.API.UnitTests.UnitTests.Auth.Providers;

public class AddClientTest
{
    [Test]
    public async Task Should_Add_Client_Successful(CancellationToken cancellationToken)
    {
        var addClient = Substitute.For<IAddClientService>();
        var logger = Substitute.For<ILogger<Client>>();
        var repo = Substitute.For<IAddClientRepository>();

        var client = new Client(
            Guid.NewGuid(),
            nameUser:"Yeray",
            lastNameuser: "Blanco Aldao",
            birthdate: DateTime.Today, 
            eventDate:DateTime.Today, 
            emailUser:"Yerayblanco@hotmail.com",
            phoneNumber: 650766561,
            center: "Carranque",
            trainingHours: 1,
            trainingModel: "Carranque",
            subscriptionType:"Subscription",
            createdByUserId:"b3a2f8d6-09e6-4e7b-9d26-3cc27430e98f");

        var service = new AddClientService(repo, logger);
            
       await service.AddNewClientAsync(client, cancellationToken);
       //Esta linea se la he pedido a chatGTP, no tengo ni idea de que poner ahi y estoy muy cansado.
       await repo.Received(1).AddClientAsync(client, cancellationToken);
    }
}
