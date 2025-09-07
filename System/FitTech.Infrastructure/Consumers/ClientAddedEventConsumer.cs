using AuleTech.Core.Messaging;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Commands.Auth.Register;
using FitTech.Application.Commands.Client.Add.Events;
using FitTech.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FitTech.Infrastructure.Consumers;

internal sealed class ClientAddedEventConsumer : IAuleTechConsumer<ClientAddedEvent>
{
    private readonly IAuleTechCommandHandler<RegisterCommand, Result> _commandHandler;
    private readonly IClientRepository _clientRepository;
    private readonly ILogger<ClientAddedEventConsumer> _logger;
    
    public ClientAddedEventConsumer(IAuleTechCommandHandler<RegisterCommand, Result> commandHandler, ILogger<ClientAddedEventConsumer> logger, IClientRepository clientRepository)
    {
        _commandHandler = commandHandler;
        _logger = logger;
        _clientRepository = clientRepository;
    }

    public async Task HandleAsync(ClientAddedEvent? message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering client('{Id}')", message!.Id);
        var result = await _clientRepository.GetAsync(message.Id, cancellationToken);

        var registerUserResult = await _commandHandler.HandleAsync(new RegisterCommand(result!.Value.Email, "TemporalPassword1", UserType.Client),
            cancellationToken);

        if (!registerUserResult.Succeeded)
        {
            throw new Exception(registerUserResult.Errors.First());
        }
    }
}
