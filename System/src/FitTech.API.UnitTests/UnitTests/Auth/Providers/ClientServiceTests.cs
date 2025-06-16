using FitTech.Application.Auth.Configuration;
using FitTech.Application.Auth.Services;
using FitTech.Application.Services;
using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;
using FitTech.Domain.Templates.EmailsTemplates;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Resend;

namespace FitTech.API.UnitTests.UnitTests.Auth.Providers;

public class ClientServiceTests
{
    [Test]
    public async Task Should_Add_Client_Successful(CancellationToken cancellationToken)
    {
        var repo = Substitute.For<IClientRepository>();
        var userManager = Substitute.For<UserManager<FitTechUser>>();
        
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

        var service = new ClientService(userManager,repo);
            
       await service.AddAsync(client, cancellationToken);
       //Esta linea se la he pedido a chatGTP, no tengo ni idea de que poner ahi y estoy muy cansado.
       await repo.Received(1).AddAsync(client, cancellationToken);
    }
}
