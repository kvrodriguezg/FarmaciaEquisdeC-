using Microsoft.AspNetCore.Mvc;
namespace FarmaciaEquisde.Models
{
    public class LoginCaptcha
    {
            public string Rut { get; set; }
            public string Password { get; set; }
            public string EmailVerificador { get; set; }
            public string CodigoAutenticacion { get; set; }

        [BindProperty(Name = "g-recaptcha-response")]
        public string RecaptchaResponse { get; set; }  
    }
}
