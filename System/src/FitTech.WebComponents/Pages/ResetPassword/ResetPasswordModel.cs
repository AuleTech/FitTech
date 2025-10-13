using System.ComponentModel.DataAnnotations;

namespace FitTech.WebComponents.Pages.ResetPassword;

public class ResetPasswordModel
{
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Debes confirmar tu contraseña.")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmNewPassword { get; set; } = null!;
}
