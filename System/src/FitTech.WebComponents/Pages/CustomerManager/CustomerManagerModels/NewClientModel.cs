namespace FitTech.WebComponents.Pages.CustomerManager.CustomerManagerModels;

public class NewClientModel
{
    public string NameUser { get; set; } = null!;
    public string LastNameuser { get; set; } = null!;
    public string EmailUser { get; set; } = null!;
    public DateOnly Birthdate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public int PhoneNumber { get; set; } 
    public int TrainingHours { get; set; }
    public string TrainingModel { get; set; } = string.Empty;
    public DateOnly Event { get; set; } =DateOnly.FromDateTime(DateTime.Now);
    public string Center { get; set; } = string.Empty;
    public int HorasSueños { get; set; }
    public string SubscriptionType { get; set; } = null!;
    public int Price { get; set; }
    public DateOnly Stardate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly Enddate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
}
