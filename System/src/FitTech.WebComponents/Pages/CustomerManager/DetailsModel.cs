namespace FitTech.WebComponents.Pages.CustomerManager;

public class DetailsModel
{
    public int TrainingHours { get; set; }
    public string TrainingModel { get; set; } = string.Empty;
    public DateOnly Event { get; set; } =DateOnly.FromDateTime(DateTime.Now);
    public string Center { get; set; } = string.Empty;
    
    public int HorasSueños { get; set; }
    
}
