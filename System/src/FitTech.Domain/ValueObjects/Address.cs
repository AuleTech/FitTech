using FitTech.Domain.Seedwork;

namespace FitTech.Domain.ValueObjects;

public class Address : ValueObject
{
    public string City { get; internal init; } = null!;
    public string Country { get; internal init; } = null!;
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return City;
        yield return Country;
    }
}
