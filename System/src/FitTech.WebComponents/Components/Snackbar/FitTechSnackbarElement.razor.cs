using Microsoft.AspNetCore.Components;

namespace FitTech.WebComponents.Components.Snackbar;

public partial class FitTechSnackbarElement : ComponentBase
{
    [Parameter] public FitTechSnackbar Snackbar { get; set; } = null!;
    public string DivCss => $"relative min-w-80 p-2 rounded-lg {_snackBarCss[Snackbar.Type]}";
    public string IconCss => $"absolute right-2 top-1/2 -translate-y-1/2 h-6 {_iconCss[Snackbar.Type]}";
    
    private readonly Dictionary<SnackbarType, string> _snackBarCss = new()
    {
        [SnackbarType.Error] = "bg-fittech-red/20 border border-fittech-red",
        [SnackbarType.Success] = "bg-green-400/20 border border-green-400"
    };

    private readonly Dictionary<SnackbarType, string> _iconCss = new()
    {
        [SnackbarType.Error] = "text-fittech-red",
        [SnackbarType.Success] = "text-green-400"
    };
}
