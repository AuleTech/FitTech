using Microsoft.AspNetCore.Components;

namespace FitTech.WebComponents.Components;

public class CancellableComponent : ComponentBase, IDisposable
{
    public CancellationTokenSource _cts = new();

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
