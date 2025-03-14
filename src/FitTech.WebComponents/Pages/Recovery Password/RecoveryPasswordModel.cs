using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTech.WebComponents.Pages.Recovery_Password
{
    public class RecoveryPasswordModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Por favor ingresa un correo electrónico válido.")]
        public string Email { get; set; } = null!;
    }
}
