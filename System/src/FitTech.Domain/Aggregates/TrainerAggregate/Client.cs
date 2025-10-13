using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Seedwork;

namespace FitTech.Domain.Aggregates.TrainerAggregate;

public class Client : Entity
{
    public Guid TrainerId { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;

    private Client()
    {
        
    }
}
