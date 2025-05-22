namespace FitTech.Domain.Templates.EmailsTemplates;

public class ResetPasswordTemplate : IEmailTemplate
{
    public static ResetPasswordTemplate Create(string callbackUrl) => new ResetPasswordTemplate(callbackUrl);

    public string Subject { get; } = "Reestablece tu contraseña";
    public string MessageType { get; } = "Reset Password";
    
    private readonly string _callBackUrl;
    
    private ResetPasswordTemplate(string callbackUrl)
    {
        _callBackUrl = callbackUrl;
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
                <h2>Restablecer contraseña</h2>
                <p>Hemos recibido una solicitud para restablecer tu contraseña.</p>
                <p>Haz clic en el siguiente enlace para continuar:</p>
                <a href=""{_callBackUrl}"" style=""display:inline-block;padding:10px 20px;background:#007bff;color:white;text-decoration:none;border-radius:5px;"">
                    Restablecer contraseña
                </a>
                <p>Si no solicitaste este cambio, puedes ignorar este mensaje.</p>
            </div>
        </body>
        </html>";
    }
}
