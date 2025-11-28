using FitTech.Domain.Enums;
using FitTech.Domain.Seedwork;

namespace FitTech.Domain.Aggregates.EmailAggregate;

public class Email : Entity, IAggregateRoot
{
    public Email(Guid externalId, string receiver, EmailType emailType, string status)
    {
        ExternalId = externalId;
        Receiver = receiver;
        EmailType = emailType;
        Status = status;
    }
    
    public Guid ExternalId { get; private set; }
    public string Receiver { get; private set; }
    public EmailType EmailType { get; private set; }
    public string Status { get; private set; }
}
