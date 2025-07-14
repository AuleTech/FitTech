using AuleTech.Core.Messaging;

namespace AuleTech.Core.Message.IntegrationTests.Rabbit.Fakes;

internal sealed class RabbitMqTestConsumer : IAuleTechConsumer<string> 
{
    public Task HandleAsync(string? message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received message {message}");
        return Task.CompletedTask;
    }
}
