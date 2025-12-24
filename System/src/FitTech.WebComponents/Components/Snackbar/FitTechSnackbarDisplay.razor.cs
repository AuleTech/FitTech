using Microsoft.AspNetCore.Components;

namespace FitTech.WebComponents.Components.Snackbar;

public partial class FitTechSnackbarDisplay
{
    [Inject] public IFitTechSnackbarService SnackbarService { get; set; } = null!;

    protected override void OnInitialized()
    {
        SnackbarService.OnSnackbarChanged += OnSnackbarChange;
    }

    private void OnSnackbarChange()
    {
        InvokeAsync(StateHasChanged);
    }
}
