namespace FitTech.Domain.Templates.EmailTemplates.Register;

public class RegisterTrainerEmailTemplate : IEmailTemplate
{
    public static RegisterTrainerEmailTemplate Create() => new RegisterTrainerEmailTemplate();

    public string Subject { get; } = "¡Bienvenido a FitTech!";
    public string MessageType { get; } = "Register Trainer";
    
    private RegisterTrainerEmailTemplate()
    {
    }
    
    
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
                <p>Te has registrado correctamente en FitTech. Estamos muy contentos de tenerte con nosotros y esperemos que te quedes por mucho</p>
                <p>Puedes acceder a la plataforma haciendo click en este enlace e ingresando tus credenciales.</p>
                <p><b>Recuerda:</b> Si no solicitaste este cambio, puedes ignorar este mensaje.</p>
            </div>
        </body>
        </html>";
    }
}
