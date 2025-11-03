using System.ComponentModel.DataAnnotations;

namespace FitTech.Trainer.Wasm.Pages.ConfigurationPage.ConfigurationModels;

public class ConfigurationUserModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(20, ErrorMessage = "El nombre no puede tener más de 20 caracteres.")]
    [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ'-]+$", ErrorMessage = "El nombre solo puede contener letras y guiones.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Por favor ingresa un correo electrónico válido.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Por favor ingresa un correo electrónico sin caracteres no permitidos.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password { get; set; } = null!;
}
