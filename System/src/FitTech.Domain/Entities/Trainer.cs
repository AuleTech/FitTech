namespace FitTech.Domain.Entities;

public class Trainer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime Birthdate { get; set; }
    public string Password { get; set; } = null!;
}
