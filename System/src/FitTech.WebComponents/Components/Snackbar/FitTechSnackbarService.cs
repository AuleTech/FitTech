using System.Collections.Concurrent;

namespace FitTech.WebComponents.Components.Snackbar;

public interface IFitTechSnackbarService
{
    FitTechSnackbar[] ShownSnackbars { get; }
    event Action? OnSnackbarChanged;
    void Add(FitTechSnackbar snackbar);
}

internal sealed class FitTechSnackbarService : IFitTechSnackbarService
{
    private readonly List<FitTechSnackbar> _snackbars = new ();

    public FitTechSnackbar[] ShownSnackbars
    {
        get
        {
            return _snackbars.Take(5).ToArray();
        }
    }

    public event Action? OnSnackbarChanged;

    public void Add(FitTechSnackbar snackbar)
    {
        snackbar.OnClose += Remove;
        _snackbars.Add(snackbar);
        OnSnackbarChanged?.Invoke();
    }

    public void Remove(FitTechSnackbar snackbar)
    {
        _snackbars.Remove(snackbar);
        OnSnackbarChanged?.Invoke();
    }
}
