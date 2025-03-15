using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTech.WebComponents.Pages.Reset_Password
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string NewPassword { get; set; } = null!;
        [Required(ErrorMessage = "Debes confirmar tu contraseña.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmNewPassword { get; set; } = null!;


    }
}
