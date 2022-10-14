using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
        public string email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string password { get; set; }
    }
}
