namespace FitTech.Domain.Templates;

public interface IEmailTemplate
{
    string Subject { get; }
    string MessageType { get; }

    string GetBody();
}
