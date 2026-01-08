using System.ComponentModel.DataAnnotations;

namespace FitTech.Client.Mobile.Pages.Auth.Registration.Models;

public class PersonalInformationForm
{
    [Required(ErrorMessage = "Introduce tu nombre")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Introduce tus apellidos")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Introduce tu ciudad")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "Introduce tu Pais")]
    public string Country { get; set; } = null!;

    [Required(ErrorMessage = "Introduce tu teléfono")]
    public string PhoneNumber { get; set; } = null!;
    
     [Required(ErrorMessage = "Introduce tu fecha de nacimiento")]
     public DateOnly? BirthDate { get; set; }
}
