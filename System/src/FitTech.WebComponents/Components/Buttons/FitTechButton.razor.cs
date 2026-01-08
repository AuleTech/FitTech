using FitTech.WebComponents.Styles.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FitTech.WebComponents.Components.Buttons;

public partial class FitTechButton : ComponentBase
{
    private string Css => $"{Class} {FitTechButtonStyles.ColorStyles[Color]} {FitTechButtonStyles.ShapeStyles[Shape]} {_baseClass}".TrimStart();
    private string _labelCss => $"{FitTechButtonStyles.SizeStyles[Size]}";

    private string? _label
    {
        get
        {
            if (ShowLoadingLabel && !string.IsNullOrWhiteSpace(LabelOnClick))
            {
                return LabelOnClick;
            }

            return Label;
        }
    }

    private string _baseClass =
        "flex items-center justify-center gap-2 border-1 cursor-pointer duration-300 transition-transform active:scale-95 active:shadow-inner";
    
    [Parameter] public Color Color { get; set; } = Color.Primary;

    [Parameter] public Size Size { get; set; } = Size.Medium;

    [Parameter] public Shape Shape { get; set; } = Shape.Default;

    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter] public string Class { get; set; } = string.Empty;

    [Parameter] public ButtonType Type { get; set; } = ButtonType.Text;

    [Parameter] public string? Label { get; set; }
    [Parameter] public string? LabelOnClick { get; set; }
    [Parameter] public string? Icon { get; set; }
    [Parameter] public bool ShowLoadingLabel { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; } = new Dictionary<string, object>();

    [Parameter] public bool Disabled { get; set; }
    
    private async Task HandleClickAsync(MouseEventArgs e)
    {
        await OnClick.InvokeAsync(e);
    }
}
