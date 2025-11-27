using FitTech.Domain.Enums;

namespace FitTech.Domain.Templates;

public interface IEmailTemplate
{
    string Subject { get; }
    EmailType EmailType { get; }

    string GetBody();
}
