namespace FitTech.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }
    public string NameUser { get; set; }
    public string LastNameuser { get; set; }
    public string EmailUser { get; set; }
    public DateOnly Birthdate { get; set; }
    public int PhoneNumber { get; set; } 
    public int TrainingHours { get; set; }
    public string TrainingModel { get; set; }
    public DateOnly EventDate { get; set; } 
    public string Center { get; set; }
    public string SubscriptionType { get; set; } 
    
    public Client(Guid id, string nameUser, string lastNameuser, DateOnly eventDate, string emailUser, DateOnly birthdate, int phoneNumber, string center, int trainingHours, string trainingModel, string subscriptionType)
    {
        
        NameUser = nameUser;
        LastNameuser = lastNameuser;
        EmailUser = emailUser;
        Birthdate = birthdate;
        PhoneNumber = phoneNumber;
        TrainingHours = trainingHours;
        TrainingModel = trainingModel;
        Center = center;
        SubscriptionType = subscriptionType;
        EventDate = eventDate;
    }

}
