using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuleTech.Core.Messaging;

public class AuleTechListenerInitializer : BackgroundService
{
    private readonly IEnumerable<IAuleTechQueueListener> _listeners;
    private readonly ILogger<AuleTechListenerInitializer> _logger;

    public AuleTechListenerInitializer(IEnumerable<IAuleTechQueueListener> listeners,
        ILogger<AuleTechListenerInitializer> logger)
    {
        _listeners = listeners;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var listener in _listeners)
        {
            _logger.LogInformation("Starting {ListenerType}...", listener.GetType().Name);
            await listener.StartAsync(stoppingToken);
        }
    }
}
