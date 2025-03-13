using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTech.WebComponents.Pages.Register
{
   public class RegisterModel
    {
       
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;//Necesario para el registro???
        public string Address { get; set; } = null!;//Necesario para el registro???
        public string City { get; set; } = null!; //Necesario para el registro???
        public string State { get; set; } = null!;//Necesario para el registro???
        public string ZipCode { get; set; } = null!;//Necesario para el registro???
        public string Country { get; set; } = null!;//Necesario para el registro???
    }
}
