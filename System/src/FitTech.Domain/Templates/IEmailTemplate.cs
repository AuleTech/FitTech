namespace FitTech.Domain.Templates;

public interface IEmailTemplate 
{
    public string Subject { get; }
    public string MessageType { get; }

    string GetBody();
}
