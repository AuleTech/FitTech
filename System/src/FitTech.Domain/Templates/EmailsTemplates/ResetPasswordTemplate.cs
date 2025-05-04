
namespace FitTech.Domain.Templates.EmailsTemplates;

public class ResetPasswordTemplate
{
    public string Subject { get; set; } = "Reset your FitTech Password";
    public string HtmlBody(ResetPasswordEmailModel model)
    {
        return $@"
        <html>
        <body style=""font-family: Arial, sans-serif;"">
            <h2>Restablecer contraseña</h2>
            <p>Hemos recibido una solicitud para restablecer tu contraseña.</p>
            <p>Haz clic en el siguiente enlace para continuar:</p>
            <a href=""{model.CallbackUrl}"" style=""display:inline-block;padding:10px 20px;background:#007bff;color:white;text-decoration:none;border-radius:5px;"">
                Restablecer contraseña
            </a>
            <p>Si no solicitaste este cambio, puedes ignorar este mensaje.</p>
        </body>
        </html>";
    }
}
