namespace FitTech.WebComponents.Components.Snackbar;

public class FitTechSnackbar
{
    public string Message = null!;
    public SnackbarType Type = SnackbarType.Error;
    public event Action<FitTechSnackbar>? OnClose;

    public void Close()
    {
        OnClose?.Invoke(this);
    }
}
