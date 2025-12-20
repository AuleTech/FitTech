using FitTech.WebComponents.Styles.Enums;

namespace FitTech.WebComponents.Components.Buttons;

public static class FitTechButtonStyles
{
    internal static Dictionary<Color, string> ColorStyles = new()
    {
        [Color.Primary] =
            "bg-fittech-green text-fittech-dark border-fittech-dark hover:bg-fittech-dark hover:text-fittech-green hover:border-fittech-green",
        [Color.Secondary] =
            "bg-fittech-dark text-fittech-green border-fittech-green hover:bg-fittech-green hover:text-fittech-dark hover:border-fittech-dark",
        [Color.Error] =
            "bg-fittech-red text-fittech-dark border-fittech-dark hover:bg-fittech-dark hover:text-fittech-red hover:border-fittech-red",
        [Color.Tertiary] =
            "bg-fittech-yellow text-fittech-dark border-fittech-dark hover:bg-fittech-dark hover:text-fittech-red hover:border-fittech-red"
    };

    internal static Dictionary<Shape, string> ShapeStyles = new()
    {
        [Shape.Default] = "rounded-lg px-[6px] py-[3px]", [Shape.Circle] = "rounded-full px-2 py-2", [Shape.Square] = "rounded-lg px-2 py-2"
    };

    internal static Dictionary<Size, string> SizeStyles = new()
    {
        [Size.Small] = "text-xs", [Size.Medium] = "text-sm", [Size.Large] = "text-xl"
    };
}
