using FitTech.Domain.Enums;

namespace FitTech.Domain.Templates.EmailTemplates.Register;

public class RegisterClientEmailTemplate : IEmailTemplate
{
    private readonly int _code;
    private readonly string _trainerName;

    private RegisterClientEmailTemplate(int code, string trainerName)
    {
        _code = code;
        _trainerName = trainerName;
    }

    public string Subject { get; } = "¡Bienvenido a FitTech!";
    public EmailType EmailType { get; } = EmailType.ClientRegister;


    public string GetBody()
    {
        return $@"
        <html>
        <head>
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        </head>
        <body style=""font-family: Arial, sans-serif; display: flex; justify-content: center; align-items: center; min-height: 100vh; background-color: #f5f5f5; margin: 0;"">
            <div style=""max-width: 500px; width: 90%; text-align: center; background: white; padding: 30px; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1);"">
                <h2>¡Bienvenido a FitTech!</h2>
                <p>¡{_trainerName} te ha añadido a su equipo! Descárgate la app móvil usando los enlaces que encontrará al final de este email e introduce el siguiente código.</p>
                <h2>{_code}</h2>
                <p><b>Recuerda:</b> Si no solicitaste este cambio, puedes ignorar este mensaje.</p>
            </div>
        </body>
        </html>";
    }

    public static RegisterClientEmailTemplate Create(int code, string trainerName)
    {
        return new RegisterClientEmailTemplate(code, trainerName);
    }
}
