using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Models
{
    public class Category
    {
        public int id { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(maximumLength:50, ErrorMessage = "No puede ser mayor a {1} caracteres")]
        [Display(Name = "Nombre")]
        public string name { get; set; }
        [Display(Name = "Tipo de Opreación")]
        public OperationTypeEnum operationTypeId { get; set; }
        public int userId { get; set; }
    }
}
