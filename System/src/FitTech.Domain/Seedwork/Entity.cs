namespace FitTech.Domain.Seedwork;

public abstract class Entity
{
    public Guid Id { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
