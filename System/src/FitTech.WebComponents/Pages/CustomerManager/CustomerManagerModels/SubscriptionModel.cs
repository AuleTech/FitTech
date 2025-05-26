namespace FitTech.WebComponents.Pages.CustomerManager;

public class SubscriptionModel
{
    public string SubscriptionType { get; set; } = null!;
    
    public int Price { get; set; }
    
    public DateOnly Stardate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    
    public DateOnly Enddate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
}
