using FitTech.Domain.Seedwork;

namespace FitTech.Domain.Aggregates.EmailAggregate;

public class Email : Entity, IAggregateRoot
{
    public Email(Guid externalId, string toEmail, string typeMessage, string emailStatus)
    {
        ExternalId = externalId;
        ToEmail = toEmail;
        TypeMessage = typeMessage;
        EmailStatus = emailStatus;
    }
    
    public Guid ExternalId { get; set; }
    public string ToEmail { get; set; }
    public string TypeMessage { get; set; }
    public string EmailStatus { get; set; }
}
