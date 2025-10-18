using FitTech.Domain.Enums;

namespace FitTech.Domain.Templates;

public interface IEmailTemplate
{
    string Subject { get; }
    MessageType MessageType { get; }

    string GetBody();
}
