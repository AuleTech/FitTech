﻿using System.ComponentModel.DataAnnotations;

namespace FitTech.WebComponents.Pages.ForgotPassword;

public class ForgotPasswordModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Por favor ingresa un correo electrónico válido.")]
    public string Email { get; set; } = null!;
}
