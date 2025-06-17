namespace FitTech.Application.Dtos.Client;

public class AddClientDto
{
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime Birthdate { get; set; }
    public int TrainingHours { get; set; }
    public string TrainingModel { get; set; } = null!;
    public DateTime EventDate { get; set; } 
    public string Center { get; set; } = null!;
    public string SubscriptionType { get; set; } = null!;
}
