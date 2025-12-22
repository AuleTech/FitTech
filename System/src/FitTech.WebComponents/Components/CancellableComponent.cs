using Microsoft.AspNetCore.Components;

namespace FitTech.WebComponents.Components;

public class CancellableComponent : ComponentBase, IDisposable
{
    public readonly CancellationTokenSource Cts = new();

    public void Dispose()
    {
        Cts.Cancel();
        Cts.Dispose();
    }
}
