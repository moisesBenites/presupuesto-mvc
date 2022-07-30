using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Models
{
    public class Account
    {
        public int id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [Display(Name = "Nombre")]
        [Remote(action: "VerifyExistsAccount", controller: "Account")] // Sirve para llamar metodo en controlador, javascript
        public string name { get; set; }
        public int userId { get; set; }
        public int orderId { get; set; }
    }
}
