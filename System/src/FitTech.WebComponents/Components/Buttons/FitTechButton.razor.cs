using FitTech.WebComponents.Styles.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FitTech.WebComponents.Components.Buttons;

public partial class FitTechButton : ComponentBase
{
    private string Css => $"{Class} {FitTechButtonStyles.ColorStyles[Color]} {FitTechButtonStyles.ShapeStyles[Shape]} {_baseClass}".TrimStart();
    private string _labelCss => $"{FitTechButtonStyles.SizeStyles[Size]}";

    private string _baseClass =
        "flex items-center justify-center gap-2 border-1 transition-all duration-300";
    
    [Parameter] public Color Color { get; set; }

    [Parameter] public Size Size { get; set; } = Size.Medium;

    [Parameter] public Shape Shape { get; set; } = Shape.Default;

    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter] public string Class { get; set; } = string.Empty;

    [Parameter] public string? Label { get; set; }
    [Parameter] public string? Icon { get; set; }
}
