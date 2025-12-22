using System.ComponentModel.DataAnnotations;

namespace FitTech.Client.Mobile.Pages.Auth.Registration.Models;

public class CredentialsForm : IValidatableObject
{
    [Required(ErrorMessage = "Introduce la contraseña")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Introduce la contraseña")]
    public string RepeatPassword { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Password != RepeatPassword)
        {
            yield return new ValidationResult(
                "Las contraseñas no coinciden",
                [nameof(RepeatPassword)]
            );
        }
    }
}
