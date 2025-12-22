using System.ComponentModel.DataAnnotations;

namespace FitTech.Client.Mobile.Pages.Auth.Registration.Models;

public class ValidationForm
{
    [Required(ErrorMessage = "Introduce el código")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "Introduce tu email")]
    public string Email { get; set; } = null!;
    
    public Guid InvitationId { get; set; }
}
