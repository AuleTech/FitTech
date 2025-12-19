namespace FitTech.WebComponents.Components.Inputs.Models;

public class FitTechSelectOption
{
    public FitTechSelectOption(string value, string? displayValue =null)
    {
        Value = value;
        DisplayValue = displayValue;
    }
    public string Value { get; set; }
    public string? DisplayValue { get; set; }
}
