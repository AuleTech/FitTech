namespace FitTech.WebComponents.Pages.CustomerManager;

public class CustomerModel
{
    public string NameUser { get; set; } = null!;
    
    public string LastNameuser { get; set; } = null!;
    
    public string EmailUser { get; set; } = null!;
    
    public DateOnly Birthdate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public int PhoneNumber { get; set; } 

}
