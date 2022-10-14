using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Models
{
    public class Transactions
    {
        public int id { get; set; }
        public int userId { get; set; }
		[Display(Name = "Fecha de Transacción")]
		[DataType(DataType.Date)]
        public DateTime date { get; set; } = DateTime.Parse(DateTime.Now.ToString("g"));
		[Display(Name = "Monto")]
        public decimal amount { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]
		[Display(Name = "Categoría")]
        public int categoryId { get; set; }
        [StringLength(maximumLength: 1000, ErrorMessage = "La nota no puede pasar de {1} caracteres")]
		[Display(Name = "Nota")]
        public string note { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
		[Display(Name = "Cuenta")]
        public int cuentaId { get; set; }
        [Display(Name = "Tipo de Operación")]
        public OperationTypeEnum operationTypeId { get; set; } = OperationTypeEnum.Entry;
        public string cuenta { get; set; }
        public string category { get; set; }
    }
}
