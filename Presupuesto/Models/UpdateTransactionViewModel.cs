namespace Presupuesto.Models
{
	public class UpdateTransactionViewModel : CreateTransactionViewModel
	{
		public int cuentaAnteriorId { get; set; }
		public decimal montoAnterior { get; set; }
		public string url { get; set; }
	}
}