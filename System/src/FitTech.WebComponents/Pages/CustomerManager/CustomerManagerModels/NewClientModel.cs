namespace FitTech.WebComponents.Pages.CustomerManager.CustomerManagerModels;

public class NewClientModel
{
    public string NameUser { get; set; } = null!;
    public string LastNameuser { get; set; } = null!;
    public string EmailUser { get; set; } = null!;
    public DateTime Birthdate { get; set; }
    public int PhoneNumber { get; set; } 
    public int TrainingHours { get; set; }
    public string TrainingModel { get; set; } = string.Empty;
    public DateTime Event { get; set; }
    public string Center { get; set; } = string.Empty;
    public string SubscriptionType { get; set; } = null!;
   
}
