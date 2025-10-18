using FitTech.Domain.Enums;
using FitTech.Domain.Seedwork;

namespace FitTech.Domain.Aggregates.EmailAggregate;

public class Email : Entity, IAggregateRoot
{
    public Email(Guid externalId, string receiver, MessageType messageType, string status)
    {
        ExternalId = externalId;
        Receiver = receiver;
        MessageType = messageType;
        Status = status;
    }
    
    public Guid ExternalId { get; private set; }
    public string Receiver { get; private set; }
    public MessageType MessageType { get; private set; }
    public string Status { get; private set; }
}
