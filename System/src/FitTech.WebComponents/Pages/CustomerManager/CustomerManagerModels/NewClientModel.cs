using System.ComponentModel.DataAnnotations;

namespace FitTech.WebComponents.Pages.CustomerManager.CustomerManagerModels;

public class NewClientModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(20, ErrorMessage = "El nombre no puede tener más de 20 caracteres.")]
    [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ'-]+$", ErrorMessage = "El nombre solo puede contener letras y guiones.")] 
    public string NameUser { get; set; } = null!;
    
    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(20, ErrorMessage = "El apellido no puede tener más de 20 caracteres.")]
    [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ' \\-]+$", ErrorMessage = "El apellido solo puede contener letras, guiones y espacios.")]
    public string LastNameuser { get; set; } = null!;
    
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Por favor ingresa un correo electrónico válido.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Por favor ingresa un correo electrónico sin caracteres no permitidos.")]
    public string EmailUser { get; set; } = null!;
    public DateTime Birthdate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "El número de telefono es obligatorio.")]
    [StringLength(9, ErrorMessage = "El teléfono no puede tener más de 9 dígitos.")]
    public int? PhoneNumber { get; set; } = null;

    [Required(ErrorMessage = "Las horas semanas son obligatorias.")]
    [StringLength(2, ErrorMessage = "No se pueden asignar mas de 99 horas semanales.")]
    public int? TrainingHours { get; set; } = null;
    
    [Required(ErrorMessage = "Es obligatorio indicar la modalidad de entrenamiento")]
    public string TrainingModel { get; set; } = null!;
    public DateTime Event { get; set; } = DateTime.Today;
    
    [StringLength(20, ErrorMessage = "El Centro deportivo no puede contener mas de 20 caracteres.")]
    public string Center { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Es obligatorio indicar el tipo de subscripción contratada")]
    public string SubscriptionType { get; set; } = null!;
   
}
