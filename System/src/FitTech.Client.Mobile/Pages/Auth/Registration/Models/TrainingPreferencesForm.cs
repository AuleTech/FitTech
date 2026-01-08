using System.ComponentModel.DataAnnotations;
using FitTech.ApiClient.Generated;

namespace FitTech.Client.Mobile.Pages.Auth.Registration.Models;

public class TrainingPreferencesForm
{
    [Required(ErrorMessage = "Introduce tu disponibilidad")]
    public int? AvailableDays { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Selecciona al menos 1 opción")]
    public string[] FavouriteExercises { get; set; } = [];

    [Required(ErrorMessage = "Selecciona tu objetivo")]
    public TrainingGoalEnumDto? Goal { get; set; }
}
