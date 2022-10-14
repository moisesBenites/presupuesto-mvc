using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Models
{
    public class CreateTransactionViewModel : Transactions
    {
        public IEnumerable<SelectListItem> Cuentas { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        //[Display(Name = "Tipo de Operación")]
        //public OperationTypeEnum OperationTypeId { get; set; }
	}
}
