using System.ComponentModel.DataAnnotations;

namespace FitTech.Client.Mobile.Pages.Auth.Registration.Models;

public class BodyMeasuresForm
{
    [Required(ErrorMessage = "Introduzca medida cadera")]
    public decimal? Hip { get; set; }
    [Required(ErrorMessage = "Introduzca perímetro de muslo")]
    public decimal? MaxThigh { get; set; } //Muslo
    [Required(ErrorMessage = "Introduce perímetro biceps")]
    public decimal? Biceps { get; set; }
    [Required(ErrorMessage = "Introduce contorno de hombros")]
    public decimal? XShoulders { get; set; } //Contorno de hombros
    [Required(ErrorMessage = "Introduce medida pecho")]
    public decimal? Chest { get; set; }
    [Required(ErrorMessage = "Introduce altura")]
    public decimal? Height { get; set; }
    [Required(ErrorMessage = "Introduce peso")]
    public decimal? Weight { get; set; }
}
