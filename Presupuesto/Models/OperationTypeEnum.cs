using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Models
{
    public enum OperationTypeEnum
    {
        [Display(Name = "Ingreso")]
        Entry = 1,
        [Display(Name = "Gasto")]
        Bill = 2,
    }
}
