using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }
    public Guid TrainerId { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTimeOffset Birthdate { get; set; }
    public int TrainingHours { get; set; }
    public string TrainingModel { get; set; } = null!;
    public DateTimeOffset EventDate { get; set; } 
    public string Center { get; set; } = null!;
    public string SubscriptionType { get; set; } = null!;
    
    public virtual FitTechUser Trainer { get; set; } = null!;
}
