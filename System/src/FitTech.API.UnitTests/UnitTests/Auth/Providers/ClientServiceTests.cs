using FitTech.Application.Auth.Configuration;
using FitTech.Application.Auth.Services;
using FitTech.Application.Dtos.Client;
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

        var service = new ClientService(userManager,repo);
            
       await service.AddAsync(new AddClientDto(), cancellationToken);
       await repo.Received(1).AddAsync(new Client(), cancellationToken);
    }
}
