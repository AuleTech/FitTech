namespace FitTech.WebComponents.Components.Inputs.Models;

public class FitTechSelectOption<T>
{
    public FitTechSelectOption(T value, string displayValue)
    {
        Value = value;
        DisplayValue = displayValue;
    }
    public T Value { get; set; }
    public string DisplayValue { get; set; }
}
